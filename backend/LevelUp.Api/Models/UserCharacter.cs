namespace LevelUp.Api.Models;

public class UserCharacter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? CharacterName { get; set; }

    /// <summary>Karakterens udseende, f.eks. som JSON.</summary>
    public string? Appearance { get; set; }

    public AppUser? User { get; set; }
}
