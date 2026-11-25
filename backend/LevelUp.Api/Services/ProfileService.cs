using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();
    Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest request);
    Task<List<PointTransactionDto>> GetTransactionsAsync();
    Task<SettingsDto> GetSettingsAsync();
    Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsRequest request);
}

public class ProfileService(AppDbContext db, ICurrentUserService currentUser) : IProfileService
{
    public async Task<ProfileDto> GetProfileAsync()
    {
        var user = await db.Users.SingleAsync(u => u.Id == currentUser.UserId);
        return await ToDtoAsync(user);
    }

    public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = await db.Users.SingleAsync(u => u.Id == currentUser.UserId);

        user.DisplayName = request.DisplayName.Trim();
        await db.SaveChangesAsync();

        return await ToDtoAsync(user);
    }

    public async Task<List<PointTransactionDto>> GetTransactionsAsync()
    {
        var transactions = await db.PointTransactions
            .Where(t => t.UserId == currentUser.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions
            .Select(t => new PointTransactionDto(t.Id, t.Amount, t.Type, t.TaskId, t.ShopItemId, t.CreatedAt))
            .ToList();
    }

    public async Task<SettingsDto> GetSettingsAsync() =>
        SettingsDto.FromEntity(await GetOrCreateSettingsAsync());

    public async Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsRequest request)
    {
        var settings = await GetOrCreateSettingsAsync();

        settings.EmailEnabled = request.EmailEnabled;
        settings.SmsEnabled = request.SmsEnabled;
        settings.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        settings.TimeZone = request.TimeZone;

        await db.SaveChangesAsync();

        return SettingsDto.FromEntity(settings);
    }

    private async Task<UserSettings> GetOrCreateSettingsAsync()
    {
        var settings = await db.UserSettings.SingleOrDefaultAsync(s => s.UserId == currentUser.UserId);

        if (settings is null)
        {
            settings = new UserSettings { UserId = currentUser.UserId };
            db.UserSettings.Add(settings);
            await db.SaveChangesAsync();
        }

        return settings;
    }

    private async Task<ProfileDto> ToDtoAsync(AppUser user)
    {
        var level = LevelCalculator.FromTotalXp(user.TotalXp);

        var completedTasks = await db.Tasks
            .CountAsync(t => t.UserId == user.Id && t.Status == TaskItemStatus.Completed);

        var inventoryCount = await db.UserInventory.CountAsync(i => i.UserId == user.Id);

        return new ProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            level.Level,
            user.TotalXp,
            level.XpIntoLevel,
            level.XpForNextLevel,
            user.CoinBalance,
            completedTasks,
            inventoryCount,
            user.CreatedAt);
    }
}
