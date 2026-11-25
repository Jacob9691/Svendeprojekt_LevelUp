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
    DateTime CreatedAt)
{
    public static TaskDto FromEntity(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.ScheduledStart,
        task.ScheduledEnd,
        task.Status,
        task.CompletedAt,
        task.CreatedAt);
}

/// <summary>Bruges både til at oprette og redigere en opgave.</summary>
public class SaveTaskRequest : IValidatableObject
{
    [Required, MaxLength(150)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Required]
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

public record CompleteTaskResponse(
    TaskDto Task,
    int XpGained,
    int PointsGained,
    int TotalXp,
    int CoinBalance,
    int Level,
    bool LeveledUp);
