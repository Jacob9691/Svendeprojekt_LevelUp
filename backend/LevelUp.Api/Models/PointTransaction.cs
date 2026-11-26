namespace LevelUp.Api.Models;

/// <summary>
/// Log over alle ændringer i brugerens points – både belønninger og køb.
/// </summary>
public class PointTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public int? ShopItemId { get; set; }

    /// <summary>Positiv ved belønning, negativ ved køb.</summary>
    public int Amount { get; set; }
    public PointTransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser? User { get; set; }
    public TaskItem? Task { get; set; }
    public ShopItem? ShopItem { get; set; }
}

public enum PointTransactionType
{
    TaskReward,
    Purchase,
    ChallengeReward,
}
