namespace LevelUp.Api.Models;

/// <summary>
/// Indstillinger for påmindelser. Telefonnummeret ligger på AppUser (fra Identity),
/// fordi det skal bekræftes med en SMS-kode før det kan bruges.
/// </summary>
public class UserSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; }
    public string TimeZone { get; set; } = "Europe/Copenhagen";

    public AppUser? User { get; set; }
}
