namespace LevelUp.Api.Models;

/// <summary>En påmindelse der sendes af BackgroundService'en.</summary>
public class Reminder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public ReminderChannel Channel { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public DateTime? SentAt { get; set; }

    public AppUser? User { get; set; }
    public TaskItem? Task { get; set; }
}

public enum ReminderChannel
{
    Email,
    Sms,
}

public enum ReminderStatus
{
    Pending,
    Sent,
    Skipped,
    Failed,
}
