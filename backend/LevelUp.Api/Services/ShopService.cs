using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IShopService
{
    Task<List<ShopItemDto>> GetItemsAsync();
    Task<PurchaseResponse> PurchaseAsync(int shopItemId);
}

public class ShopService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IAchievementService achievementService) : IShopService
{
    public async Task<List<ShopItemDto>> GetItemsAsync()
    {
        var ownedIds = await db.UserInventory
            .Where(i => i.UserId == currentUser.UserId)
            .Select(i => i.ShopItemId)
            .ToListAsync();

        var items = await db.ShopItems
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Price)
            .ToListAsync();

        return items
            .Select(i => new ShopItemDto(i.Id, i.Name, i.Description, i.Price, i.Category, ownedIds.Contains(i.Id)))
            .ToList();
    }

    public async Task<PurchaseResponse> PurchaseAsync(int shopItemId)
    {
        var item = await db.ShopItems.FindAsync(shopItemId)
            ?? throw new NotFoundException($"Genstand {shopItemId} findes ikke i shoppen.");

        var user = await db.Users.SingleAsync(u => u.Id == currentUser.UserId);

        var alreadyOwned = await db.UserInventory
            .AnyAsync(i => i.UserId == user.Id && i.ShopItemId == item.Id);

        if (alreadyOwned)
        {
            throw new BusinessRuleException($"Du ejer allerede '{item.Name}'.");
        }

        if (user.CoinBalance < item.Price)
        {
            throw new BusinessRuleException(
                $"Du har ikke nok points. '{item.Name}' koster {item.Price}, du har {user.CoinBalance}.");
        }

        user.CoinBalance -= item.Price;

        var inventoryItem = new UserInventoryItem
        {
            UserId = user.Id,
            ShopItemId = item.Id,
        };
        db.UserInventory.Add(inventoryItem);

        db.PointTransactions.Add(new PointTransaction
        {
            UserId = user.Id,
            ShopItemId = item.Id,
            Amount = -item.Price,
            Type = PointTransactionType.Purchase,
        });

        await db.SaveChangesAsync();

        var unlocked = await achievementService.UnlockNewAsync();

        var dto = new InventoryItemDto(
            inventoryItem.Id, item.Id, item.Name, item.Description, item.Category,
            inventoryItem.IsEquipped, inventoryItem.PurchasedAt);

        return new PurchaseResponse(dto, user.CoinBalance, unlocked);
    }
}
