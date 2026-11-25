using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetInventoryAsync();
    Task<InventoryItemDto> EquipAsync(int inventoryItemId);
    Task<InventoryItemDto> UnequipAsync(int inventoryItemId);
}

public class InventoryService(AppDbContext db, ICurrentUserService currentUser) : IInventoryService
{
    public async Task<List<InventoryItemDto>> GetInventoryAsync()
    {
        var items = await db.UserInventory
            .Include(i => i.ShopItem)
            .Where(i => i.UserId == currentUser.UserId)
            .OrderBy(i => i.ShopItem!.Category)
            .ThenBy(i => i.PurchasedAt)
            .ToListAsync();

        return items.Select(ToDto).ToList();
    }

    public async Task<InventoryItemDto> EquipAsync(int inventoryItemId)
    {
        var item = await FindOwnItemAsync(inventoryItemId);

        // Kun én genstand pr. kategori kan være udstyret ad gangen
        var category = item.ShopItem!.Category;
        var equippedInSameCategory = await db.UserInventory
            .Where(i => i.UserId == currentUser.UserId
                        && i.IsEquipped
                        && i.Id != item.Id
                        && i.ShopItem!.Category == category)
            .ToListAsync();

        foreach (var other in equippedInSameCategory)
        {
            other.IsEquipped = false;
        }

        item.IsEquipped = true;
        await db.SaveChangesAsync();

        return ToDto(item);
    }

    public async Task<InventoryItemDto> UnequipAsync(int inventoryItemId)
    {
        var item = await FindOwnItemAsync(inventoryItemId);

        item.IsEquipped = false;
        await db.SaveChangesAsync();

        return ToDto(item);
    }

    private async Task<UserInventoryItem> FindOwnItemAsync(int id) =>
        await db.UserInventory
            .Include(i => i.ShopItem)
            .SingleOrDefaultAsync(i => i.Id == id && i.UserId == currentUser.UserId)
        ?? throw new NotFoundException($"Genstand {id} blev ikke fundet i dit inventar.");

    private static InventoryItemDto ToDto(UserInventoryItem item) => new(
        item.Id,
        item.ShopItemId,
        item.ShopItem!.Name,
        item.ShopItem.Description,
        item.ShopItem.Category,
        item.IsEquipped,
        item.PurchasedAt);
}
