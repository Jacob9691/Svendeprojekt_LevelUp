using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IChallengeService
{
    Task<List<ChallengeDto>> GetChallengesAsync();

    /// <summary>Starter udfordringen og lægger dens opgaver i kalenderen som en serie.</summary>
    Task<ChallengeDto> StartAsync(int challengeId, StartChallengeRequest request);

    /// <summary>Opgiver en aktiv udfordring. Ikke-gennemførte opgaver fjernes fra kalenderen.</summary>
    Task AbandonAsync(int challengeId);

    /// <summary>
    /// Kaldes når en opgave er gennemført. Tæller udfordringen op, hvis opgaven hører til en,
    /// og giver bonus, når den er færdig. Gemmer ikke – kalderen kalder SaveChanges.
    /// </summary>
    Task<ChallengeProgressDto?> RegisterCompletionAsync(TaskItem task, AppUser user);
}

public class ChallengeService(AppDbContext db, ICurrentUserService currentUser) : IChallengeService
{
    public async Task<List<ChallengeDto>> GetChallengesAsync()
    {
        var challenges = await db.Challenges.OrderBy(c => c.RequiredCount).ToListAsync();
        var userChallenges = await db.UserChallenges
            .Where(uc => uc.UserId == currentUser.UserId)
            .ToDictionaryAsync(uc => uc.ChallengeId);

        return challenges
            .Select(c => ToDto(c, userChallenges.GetValueOrDefault(c.Id)))
            .ToList();
    }

    public async Task<ChallengeDto> StartAsync(int challengeId, StartChallengeRequest request)
    {
        var challenge = await db.Challenges.FindAsync(challengeId)
            ?? throw new NotFoundException($"Udfordring {challengeId} findes ikke.");

        if (challenge.EndDate is not null && challenge.EndDate < DateTime.Today)
        {
            throw new BusinessRuleException($"'{challenge.Name}' er udløbet og kan ikke startes.");
        }

        var existing = await db.UserChallenges
            .SingleOrDefaultAsync(uc => uc.UserId == currentUser.UserId && uc.ChallengeId == challengeId);

        if (existing is not null)
        {
            throw new BusinessRuleException(existing.CompletedAt is null
                ? $"Du er allerede i gang med '{challenge.Name}'."
                : $"Du har allerede gennemført '{challenge.Name}'.");
        }

        var start = request.ScheduledStart!.Value;
        var duration = request.ScheduledEnd - start;
        var seriesId = Guid.NewGuid();

        var tasks = Recurrence.Count(start, challenge.Repeat, challenge.RequiredCount)
            .Select(occurrenceStart => new TaskItem
            {
                UserId = currentUser.UserId,
                Title = challenge.TaskTitle,
                Description = $"Udfordring: {challenge.Name}",
                ScheduledStart = occurrenceStart,
                ScheduledEnd = occurrenceStart + duration,
                SeriesId = seriesId,
            });

        var userChallenge = new UserChallenge
        {
            UserId = currentUser.UserId,
            ChallengeId = challenge.Id,
            SeriesId = seriesId,
            StartedAt = DateTime.UtcNow,
        };

        db.Tasks.AddRange(tasks);
        db.UserChallenges.Add(userChallenge);
        await db.SaveChangesAsync();

        return ToDto(challenge, userChallenge);
    }

    public async Task AbandonAsync(int challengeId)
    {
        var userChallenge = await db.UserChallenges
            .SingleOrDefaultAsync(uc => uc.UserId == currentUser.UserId && uc.ChallengeId == challengeId)
            ?? throw new NotFoundException("Du er ikke i gang med den udfordring.");

        if (userChallenge.CompletedAt is not null)
        {
            throw new BusinessRuleException("En gennemført udfordring kan ikke opgives.");
        }

        // Gennemførte opgaver bliver (de har givet XP), resten fjernes fra kalenderen
        var pendingIds = await db.Tasks
            .Where(t => t.UserId == currentUser.UserId
                        && t.SeriesId == userChallenge.SeriesId
                        && t.Status != TaskItemStatus.Completed)
            .Select(t => t.Id)
            .ToListAsync();

        await TaskService.DeleteTasksAsync(db, pendingIds);

        db.UserChallenges.Remove(userChallenge);
        await db.SaveChangesAsync();
    }

    public async Task<ChallengeProgressDto?> RegisterCompletionAsync(TaskItem task, AppUser user)
    {
        if (task.SeriesId is null)
        {
            return null;
        }

        var userChallenge = await db.UserChallenges
            .Include(uc => uc.Challenge)
            .SingleOrDefaultAsync(uc => uc.UserId == user.Id
                                        && uc.SeriesId == task.SeriesId
                                        && uc.CompletedAt == null);

        if (userChallenge is null)
        {
            return null;
        }

        var challenge = userChallenge.Challenge!;
        userChallenge.Progress++;

        var completed = userChallenge.Progress >= challenge.RequiredCount;

        if (completed)
        {
            userChallenge.CompletedAt = DateTime.UtcNow;
            user.TotalXp += challenge.RewardXp;
            user.CoinBalance += challenge.RewardCoins;

            db.PointTransactions.Add(new PointTransaction
            {
                UserId = user.Id,
                Amount = challenge.RewardCoins,
                Type = PointTransactionType.ChallengeReward,
            });
        }

        return new ChallengeProgressDto(
            challenge.Id,
            challenge.Name,
            userChallenge.Progress,
            challenge.RequiredCount,
            completed,
            challenge.RewardXp,
            challenge.RewardCoins);
    }

    private static ChallengeDto ToDto(Challenge challenge, UserChallenge? userChallenge) => new(
        challenge.Id,
        challenge.Name,
        challenge.Description,
        challenge.TaskTitle,
        challenge.Repeat,
        challenge.RequiredCount,
        challenge.RewardXp,
        challenge.RewardCoins,
        challenge.EndDate,
        userChallenge switch
        {
            null => ChallengeStatus.NotStarted,
            { CompletedAt: not null } => ChallengeStatus.Completed,
            _ => ChallengeStatus.Active,
        },
        userChallenge?.Progress ?? 0,
        userChallenge?.StartedAt,
        userChallenge?.CompletedAt);
}
