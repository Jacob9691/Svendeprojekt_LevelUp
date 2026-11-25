namespace LevelUp.Api.Models;

/// <summary>En genstand brugeren har købt. Gemmes i tabellen "user_inventory".</summary>
public class UserInventoryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShopItemId { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime PurchasedAt { get; set; }

    public AppUser? User { get; set; }
    public ShopItem? ShopItem { get; set; }
}
