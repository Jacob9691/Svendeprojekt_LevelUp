namespace LevelUp.Api.Models;

public class Challenge
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int RewardCoins { get; set; }
    public int RewardXp { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UserChallenge
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ChallengeId { get; set; }
    public int Progress { get; set; }
    public DateTime? CompletedAt { get; set; }

    public AppUser? User { get; set; }
    public Challenge? Challenge { get; set; }
}
