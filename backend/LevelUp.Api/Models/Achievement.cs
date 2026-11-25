namespace LevelUp.Api.Models;

public class Achievement
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    /// <summary>Hvor mange gange noget skal gøres for at låse achievementet op.</summary>
    public int? RequiredCount { get; set; }
}

public class UserAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }

    public AppUser? User { get; set; }
    public Achievement? Achievement { get; set; }
}
