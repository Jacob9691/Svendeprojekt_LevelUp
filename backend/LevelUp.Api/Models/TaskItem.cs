namespace LevelUp.Api.Models;

/// <summary>
/// En opgave i brugerens ugeplan. Hedder TaskItem for ikke at kollidere med System.Threading.Tasks.Task,
/// men gemmes i tabellen "task".
/// </summary>
public class TaskItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser? User { get; set; }
}

public enum TaskItemStatus
{
    Pending,
    Completed,
}
