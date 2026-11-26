namespace LevelUp.Api.Models;

/// <summary>
/// En udfordring brugeren kan starte, f.eks. "Gå en tur hver dag i 7 dage".
/// Når den startes, lægges <see cref="RequiredCount"/> opgaver i kalenderen som en serie.
/// </summary>
public class Challenge
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int RewardCoins { get; set; }
    public int RewardXp { get; set; }

    /// <summary>Sidste dag udfordringen kan startes. Null = altid.</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Titlen på de opgaver der lægges i kalenderen.</summary>
    public required string TaskTitle { get; set; }

    /// <summary>Hvor ofte opgaven gentages.</summary>
    public RecurrenceFrequency Repeat { get; set; }

    /// <summary>Hvor mange opgaver der skal gennemføres.</summary>
    public int RequiredCount { get; set; }
}

public class UserChallenge
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ChallengeId { get; set; }

    /// <summary>Antal gennemførte opgaver i udfordringen.</summary>
    public int Progress { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>Serien af opgaver i kalenderen, som hører til udfordringen.</summary>
    public Guid SeriesId { get; set; }

    public AppUser? User { get; set; }
    public Challenge? Challenge { get; set; }
}
