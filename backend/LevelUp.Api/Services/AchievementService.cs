using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IAchievementService
{
    /// <summary>Alle achievements med brugerens fremskridt. Låser også nye op, hvis de er nået.</summary>
    Task<List<AchievementDto>> GetAchievementsAsync();

    /// <summary>
    /// Låser de achievements op, som brugeren nu har nået, og returnerer dem.
    /// Kaldes efter alt der kan ændre tallene: gennemført opgave, køb og udfordring.
    /// </summary>
    Task<List<AchievementDto>> UnlockNewAsync();
}

public class AchievementService(AppDbContext db, ICurrentUserService currentUser) : IAchievementService
{
    public async Task<List<AchievementDto>> GetAchievementsAsync()
    {
        await UnlockNewAsync();

        var stats = await GetStatsAsync();
        var unlocked = await db.UserAchievements
            .Where(a => a.UserId == currentUser.UserId)
            .ToDictionaryAsync(a => a.AchievementId, a => a.UnlockedAt);

        // Rækkefølgen fra SeedData (Type gemmes som tekst, så OrderBy(Type) ville sortere alfabetisk)
        var achievements = await db.Achievements
            .OrderBy(a => a.Id)
            .ToListAsync();

        return achievements
            .Select(a => ToDto(a, stats, unlocked.TryGetValue(a.Id, out var at) ? at : null))
            .ToList();
    }

    public async Task<List<AchievementDto>> UnlockNewAsync()
    {
        var stats = await GetStatsAsync();
        var unlockedIds = await db.UserAchievements
            .Where(a => a.UserId == currentUser.UserId)
            .Select(a => a.AchievementId)
            .ToListAsync();

        var reached = (await db.Achievements.Where(a => !unlockedIds.Contains(a.Id)).ToListAsync())
            .Where(a => stats.CountFor(a.Type) >= (a.RequiredCount ?? 1))
            .ToList();

        if (reached.Count == 0)
        {
            return [];
        }

        var now = DateTime.UtcNow;
        db.UserAchievements.AddRange(reached.Select(a => new UserAchievement
        {
            UserId = currentUser.UserId,
            AchievementId = a.Id,
            UnlockedAt = now,
        }));
        await db.SaveChangesAsync();

        return reached.Select(a => ToDto(a, stats, now)).ToList();
    }

    /// <summary>Brugerens tal for hver type achievement.</summary>
    private async Task<AchievementStats> GetStatsAsync()
    {
        var userId = currentUser.UserId;
        var totalXp = await db.Users.Where(u => u.Id == userId).Select(u => u.TotalXp).SingleAsync();

        return new AchievementStats(
            Level: LevelCalculator.FromTotalXp(totalXp).Level,
            // Point-historikken tælles i stedet for opgaver, så slettede opgaver stadig tæller med
            TasksCompleted: await db.PointTransactions.CountAsync(t => t.UserId == userId && t.Type == PointTransactionType.TaskReward),
            ItemsBought: await db.UserInventory.CountAsync(i => i.UserId == userId),
            ChallengesCompleted: await db.UserChallenges.CountAsync(c => c.UserId == userId && c.CompletedAt != null));
    }

    private static AchievementDto ToDto(Achievement achievement, AchievementStats stats, DateTime? unlockedAt)
    {
        var required = achievement.RequiredCount ?? 1;

        return new AchievementDto(
            achievement.Id,
            achievement.Name,
            achievement.Description,
            achievement.Type,
            required,
            Math.Min(stats.CountFor(achievement.Type), required),
            unlockedAt);
    }

    private record AchievementStats(int Level, int TasksCompleted, int ItemsBought, int ChallengesCompleted)
    {
        public int CountFor(AchievementType type) => type switch
        {
            AchievementType.Level => Level,
            AchievementType.TasksCompleted => TasksCompleted,
            AchievementType.ItemsBought => ItemsBought,
            AchievementType.ChallengesCompleted => ChallengesCompleted,
            _ => 0,
        };
    }
}
