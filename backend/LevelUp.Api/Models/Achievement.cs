namespace LevelUp.Api.Models;

/// <summary>
/// Et mål brugeren kan nå, f.eks. "Nå level 10". Låses op automatisk, når brugerens tal
/// for <see cref="Type"/> når <see cref="RequiredCount"/>.
/// </summary>
public class Achievement
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    /// <summary>Hvad der tælles.</summary>
    public AchievementType Type { get; set; }

    /// <summary>Hvor meget der skal til, f.eks. level 10 eller 50 opgaver.</summary>
    public int? RequiredCount { get; set; }
}

public enum AchievementType
{
    /// <summary>Brugerens level.</summary>
    Level,
    /// <summary>Antal gennemførte opgaver (tælles fra point-historikken, så slettede opgaver stadig tæller).</summary>
    TasksCompleted,
    /// <summary>Antal købte genstande.</summary>
    ItemsBought,
    /// <summary>Antal gennemførte udfordringer.</summary>
    ChallengesCompleted,
}

public class UserAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }

    public AppUser? User { get; set; }
    public Achievement? Achievement { get; set; }
}
