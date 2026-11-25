namespace LevelUp.Api.Dtos;

public record ShopItemDto(
    int Id,
    string Name,
    string? Description,
    int Price,
    string Category,
    bool Owned);

public record PurchaseResponse(InventoryItemDto Item, int CoinBalance);

public record InventoryItemDto(
    int Id,
    int ShopItemId,
    string Name,
    string? Description,
    string Category,
    bool IsEquipped,
    DateTime PurchasedAt);
