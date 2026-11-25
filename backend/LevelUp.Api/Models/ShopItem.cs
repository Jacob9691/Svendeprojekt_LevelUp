namespace LevelUp.Api.Models;

public class ShopItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Price { get; set; }

    /// <summary>F.eks. "Hat", "Outfit" eller "Accessory". Kun én genstand pr. kategori kan være udstyret.</summary>
    public required string Category { get; set; }
}
