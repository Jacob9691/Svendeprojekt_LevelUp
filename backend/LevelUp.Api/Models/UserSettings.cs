namespace LevelUp.Api.Models;

public class UserSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; }
    public string? PhoneNumber { get; set; }
    public string TimeZone { get; set; } = "Europe/Copenhagen";

    public AppUser? User { get; set; }
}
