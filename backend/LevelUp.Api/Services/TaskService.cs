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
    Task<TaskDto> CreateTaskAsync(SaveTaskRequest request);
    Task<TaskDto> UpdateTaskAsync(int id, SaveTaskRequest request);
    Task DeleteTaskAsync(int id);
    Task<CompleteTaskResponse> CompleteTaskAsync(int id);
}

public class TaskService(AppDbContext db, ICurrentUserService currentUser) : ITaskService
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

    public async Task<TaskDto> CreateTaskAsync(SaveTaskRequest request)
    {
        var task = new TaskItem
        {
            UserId = currentUser.UserId,
            Title = request.Title.Trim(),
            Description = request.Description,
            ScheduledStart = request.ScheduledStart!.Value,
            ScheduledEnd = request.ScheduledEnd,
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return TaskDto.FromEntity(task);
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

    public async Task DeleteTaskAsync(int id)
    {
        var task = await FindOwnTaskAsync(id);

        await using var transaction = await db.Database.BeginTransactionAsync();

        // Point-historikken bevares, men mister referencen til opgaven
        await db.PointTransactions
            .Where(t => t.TaskId == task.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.TaskId, (int?)null));

        // Påmindelser for en slettet opgave giver ikke mening
        await db.Reminders
            .Where(r => r.TaskId == task.Id)
            .ExecuteDeleteAsync();

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();

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

        await db.SaveChangesAsync();

        var levelAfter = LevelCalculator.FromTotalXp(user.TotalXp).Level;

        return new CompleteTaskResponse(
            TaskDto.FromEntity(task),
            Rewards.TaskXp,
            Rewards.TaskPoints,
            user.TotalXp,
            user.CoinBalance,
            levelAfter,
            levelAfter > levelBefore);
    }

    /// <summary>Finder en opgave, men kun hvis den tilhører den aktuelle bruger.</summary>
    private async Task<TaskItem> FindOwnTaskAsync(int id) =>
        await db.Tasks.SingleOrDefaultAsync(t => t.Id == id && t.UserId == currentUser.UserId)
        ?? throw new NotFoundException($"Opgave {id} blev ikke fundet.");
}
