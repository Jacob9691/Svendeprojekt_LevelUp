namespace LevelUp.Api.Models;

/// <summary>
/// En bruger af systemet. Bliver senere koblet på ASP.NET Core Identity
/// (e-mailbekræftelse og 2FA).
/// </summary>
public class AppUser
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string DisplayName { get; set; }
    public int TotalXp { get; set; }
    public int CoinBalance { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserSettings? Settings { get; set; }
    public UserCharacter? Character { get; set; }
    public List<TaskItem> Tasks { get; set; } = [];
    public List<WeeklyAvailability> WeeklyAvailability { get; set; } = [];
    public List<UserInventoryItem> Inventory { get; set; } = [];
    public List<PointTransaction> PointTransactions { get; set; } = [];
    public List<Reminder> Reminders { get; set; } = [];
    public List<UserAchievement> Achievements { get; set; } = [];
    public List<UserChallenge> Challenges { get; set; } = [];
}
