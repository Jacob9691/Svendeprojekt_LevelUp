using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;

namespace LevelUp.Api.Dtos;

/// <summary>Et achievement med brugerens fremskridt, f.eks. level 3 af 10.</summary>
public record AchievementDto(
    int Id,
    string Name,
    string? Description,
    AchievementType Type,
    int RequiredCount,
    int CurrentCount,
    DateTime? UnlockedAt);

public enum ChallengeStatus
{
    NotStarted,
    Active,
    Completed,
}

/// <summary>En udfordring og hvor langt brugeren er nået med den.</summary>
public record ChallengeDto(
    int Id,
    string Name,
    string? Description,
    string TaskTitle,
    RecurrenceFrequency Repeat,
    int RequiredCount,
    int RewardXp,
    int RewardCoins,
    DateTime? EndDate,
    ChallengeStatus Status,
    int Progress,
    DateTime? StartedAt,
    DateTime? CompletedAt);

/// <summary>Hvornår udfordringens opgaver skal ligge i kalenderen – den første dag og tidspunktet.</summary>
public class StartChallengeRequest : IValidatableObject
{
    [Display(Name = "Starttidspunkt")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public DateTime? ScheduledStart { get; init; }

    public DateTime? ScheduledEnd { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ScheduledEnd is not null && ScheduledStart is not null && ScheduledEnd < ScheduledStart)
        {
            yield return new ValidationResult(
                "Sluttidspunktet skal være efter starttidspunktet.",
                [nameof(ScheduledEnd)]);
        }
    }
}

/// <summary>Fremskridt i en udfordring efter at en af dens opgaver er gennemført.</summary>
public record ChallengeProgressDto(
    int ChallengeId,
    string Name,
    int Progress,
    int RequiredCount,
    bool Completed,
    int RewardXp,
    int RewardCoins);
