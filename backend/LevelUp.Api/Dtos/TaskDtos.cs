using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;

namespace LevelUp.Api.Dtos;

public record TaskDto(
    int Id,
    string Title,
    string? Description,
    DateTime ScheduledStart,
    DateTime? ScheduledEnd,
    TaskItemStatus Status,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    Guid? SeriesId)
{
    public static TaskDto FromEntity(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.ScheduledStart,
        task.ScheduledEnd,
        task.Status,
        task.CompletedAt,
        task.CreatedAt,
        task.SeriesId);
}

/// <summary>Redigering af én opgave (også hvis den er del af en serie).</summary>
public class SaveTaskRequest : IValidatableObject
{
    [Display(Name = "Titel")]
    [Required(ErrorMessage = ValidationMessages.Required), MaxLength(150, ErrorMessage = ValidationMessages.MaxLength)]
    public string Title { get; init; } = string.Empty;

    [Display(Name = "Beskrivelsen")]
    [MaxLength(2000, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Description { get; init; }

    [Display(Name = "Starttidspunkt")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    public DateTime? ScheduledStart { get; init; }

    public DateTime? ScheduledEnd { get; init; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ScheduledEnd is not null && ScheduledStart is not null && ScheduledEnd < ScheduledStart)
        {
            yield return new ValidationResult(
                "Sluttidspunktet skal være efter starttidspunktet.",
                [nameof(ScheduledEnd)]);
        }
    }
}

/// <summary>Oprettelse af en opgave – eventuelt som en gentagende serie.</summary>
public class CreateTaskRequest : SaveTaskRequest
{
    /// <summary>Hvor ofte opgaven gentages. Null = kun én gang.</summary>
    public RecurrenceFrequency? Repeat { get; init; }

    /// <summary>Sidste dato en gentagelse må ligge på (medtaget). Krævet når Repeat er sat.</summary>
    public DateOnly? RepeatUntil { get; init; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        if (Repeat is null || ScheduledStart is null)
        {
            yield break;
        }

        var startDate = DateOnly.FromDateTime(ScheduledStart.Value);

        if (RepeatUntil is null)
        {
            yield return new ValidationResult("Vælg hvornår gentagelsen skal slutte.", [nameof(RepeatUntil)]);
        }
        else if (RepeatUntil < startDate)
        {
            yield return new ValidationResult("Slutdatoen skal være efter startdatoen.", [nameof(RepeatUntil)]);
        }
        else if (RepeatUntil > startDate.AddYears(1))
        {
            yield return new ValidationResult("En serie kan højst vare et år.", [nameof(RepeatUntil)]);
        }
    }
}

/// <summary>
/// Svar når en opgave er gennemført. XpGained/PointsGained er for selve opgaven – bonus for en
/// gennemført udfordring står i Challenge. TotalXp, CoinBalance og Level er efter begge dele.
/// </summary>
public record CompleteTaskResponse(
    TaskDto Task,
    int XpGained,
    int PointsGained,
    int TotalXp,
    int CoinBalance,
    int Level,
    bool LeveledUp,
    ChallengeProgressDto? Challenge,
    List<AchievementDto> UnlockedAchievements);
