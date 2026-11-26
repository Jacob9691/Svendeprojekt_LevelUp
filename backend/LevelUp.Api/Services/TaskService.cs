using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface ITaskService
{
    Task<List<TaskDto>> GetTasksAsync(DateTime? from, DateTime? to);
    Task<TaskDto> GetTaskAsync(int id);
    /// <summary>Opretter opgaven – eller én opgave pr. gentagelse. Returnerer den første.</summary>
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request);
    Task<TaskDto> UpdateTaskAsync(int id, SaveTaskRequest request);
    /// <summary>
    /// Sletter opgaven. Er includeFollowing sand, slettes også senere, ikke-gennemførte opgaver i samme serie.
    /// </summary>
    Task DeleteTaskAsync(int id, bool includeFollowing = false);
    Task<CompleteTaskResponse> CompleteTaskAsync(int id);
}

public class TaskService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IChallengeService challengeService,
    IAchievementService achievementService) : ITaskService
{
    public async Task<List<TaskDto>> GetTasksAsync(DateTime? from, DateTime? to)
    {
        var query = db.Tasks.Where(t => t.UserId == currentUser.UserId);

        if (from is not null)
        {
            query = query.Where(t => t.ScheduledStart >= from);
        }

        if (to is not null)
        {
            query = query.Where(t => t.ScheduledStart < to);
        }

        var tasks = await query.OrderBy(t => t.ScheduledStart).ToListAsync();
        return tasks.Select(TaskDto.FromEntity).ToList();
    }

    public async Task<TaskDto> GetTaskAsync(int id) =>
        TaskDto.FromEntity(await FindOwnTaskAsync(id));

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request)
    {
        var start = request.ScheduledStart!.Value;
        var occurrences = request.Repeat is null
            ? [start]
            : Recurrence.Until(start, request.Repeat.Value, request.RepeatUntil!.Value, nameof(request.RepeatUntil));

        // Samme varighed for alle gentagelser
        var duration = request.ScheduledEnd - start;
        var seriesId = occurrences.Count > 1 ? Guid.NewGuid() : (Guid?)null;

        var tasks = occurrences
            .Select(occurrenceStart => new TaskItem
            {
                UserId = currentUser.UserId,
                Title = request.Title.Trim(),
                Description = request.Description,
                ScheduledStart = occurrenceStart,
                ScheduledEnd = occurrenceStart + duration,
                SeriesId = seriesId,
            })
            .ToList();

        db.Tasks.AddRange(tasks);
        await db.SaveChangesAsync();

        return TaskDto.FromEntity(tasks[0]);
    }

    public async Task<TaskDto> UpdateTaskAsync(int id, SaveTaskRequest request)
    {
        var task = await FindOwnTaskAsync(id);

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.ScheduledStart = request.ScheduledStart!.Value;
        task.ScheduledEnd = request.ScheduledEnd;

        await db.SaveChangesAsync();

        return TaskDto.FromEntity(task);
    }

    public async Task DeleteTaskAsync(int id, bool includeFollowing = false)
    {
        var task = await FindOwnTaskAsync(id);
        var ids = new List<int> { task.Id };

        // "Denne og fremtidige": senere gentagelser i serien. Gennemførte bevares som historik.
        if (includeFollowing && task.SeriesId is not null)
        {
            ids.AddRange(await db.Tasks
                .Where(t => t.UserId == currentUser.UserId
                            && t.SeriesId == task.SeriesId
                            && t.Id != task.Id
                            && t.ScheduledStart >= task.ScheduledStart
                            && t.Status != TaskItemStatus.Completed)
                .Select(t => t.Id)
                .ToListAsync());
        }

        await DeleteTasksAsync(db, ids);
    }

    /// <summary>
    /// Sletter opgaverne i én transaktion. Bruges også når en udfordring opgives.
    /// Kalderen skal have tjekket at opgaverne tilhører brugeren.
    /// </summary>
    public static async Task DeleteTasksAsync(AppDbContext db, List<int> ids)
    {
        if (ids.Count == 0)
        {
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync();

        // Point-historikken bevares, men mister referencen til opgaven
        await db.PointTransactions
            .Where(t => t.TaskId != null && ids.Contains(t.TaskId.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.TaskId, (int?)null));

        // Påmindelser for en slettet opgave giver ikke mening
        await db.Reminders
            .Where(r => r.TaskId != null && ids.Contains(r.TaskId.Value))
            .ExecuteDeleteAsync();

        await db.Tasks
            .Where(t => ids.Contains(t.Id))
            .ExecuteDeleteAsync();

        await transaction.CommitAsync();
    }

    public async Task<CompleteTaskResponse> CompleteTaskAsync(int id)
    {
        var task = await FindOwnTaskAsync(id);

        // Belønning gives kun én gang pr. opgave
        if (task.Status == TaskItemStatus.Completed)
        {
            throw new BusinessRuleException("Opgaven er allerede gennemført og belønnet.");
        }

        var user = await db.Users.SingleAsync(u => u.Id == currentUser.UserId);
        var levelBefore = LevelCalculator.FromTotalXp(user.TotalXp).Level;

        task.Status = TaskItemStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;

        user.TotalXp += Rewards.TaskXp;
        user.CoinBalance += Rewards.TaskPoints;

        db.PointTransactions.Add(new PointTransaction
        {
            UserId = user.Id,
            TaskId = task.Id,
            Amount = Rewards.TaskPoints,
            Type = PointTransactionType.TaskReward,
        });

        // Tæller udfordringen op (og giver bonus, hvis den bliver færdig) i samme SaveChanges
        var challenge = await challengeService.RegisterCompletionAsync(task, user);

        await db.SaveChangesAsync();

        // Achievements tjekkes efter gem, så de nye tal er med
        var unlocked = await achievementService.UnlockNewAsync();
        var levelAfter = LevelCalculator.FromTotalXp(user.TotalXp).Level;

        return new CompleteTaskResponse(
            TaskDto.FromEntity(task),
            Rewards.TaskXp,
            Rewards.TaskPoints,
            user.TotalXp,
            user.CoinBalance,
            levelAfter,
            levelAfter > levelBefore,
            challenge,
            unlocked);
    }

    /// <summary>Finder en opgave, men kun hvis den tilhører den aktuelle bruger.</summary>
    private async Task<TaskItem> FindOwnTaskAsync(int id) =>
        await db.Tasks.SingleOrDefaultAsync(t => t.Id == id && t.UserId == currentUser.UserId)
        ?? throw new NotFoundException($"Opgave {id} blev ikke fundet.");
}
