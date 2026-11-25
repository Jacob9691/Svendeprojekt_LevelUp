using Microsoft.AspNetCore.Identity;

namespace LevelUp.Api.Models;

/// <summary>
/// En bruger af systemet. Arver fra IdentityUser, som giver e-mail, password hash,
/// telefonnummer, bekræftelse, 2FA og lockout. Gemmes i tabellen "app_user".
/// </summary>
public class AppUser : IdentityUser<int>
{
    public required string DisplayName { get; set; }
    public int TotalXp { get; set; }
    public int CoinBalance { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Hvordan brugeren modtager engangskoden ved login.</summary>
    public TwoFactorMethod TwoFactorMethod { get; set; } = TwoFactorMethod.Email;

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

public enum TwoFactorMethod
{
    Email,
    Sms,
}
