using LevelUp.Api.Common;
using LevelUp.Api.Data;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using LevelUp.Api.Services.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();
    Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest request);
    Task<List<PointTransactionDto>> GetTransactionsAsync();
    Task<SettingsDto> GetSettingsAsync();
    Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsRequest request);
    Task RequestPhoneChangeAsync(ChangePhoneRequest request);
    Task<SettingsDto> ConfirmPhoneChangeAsync(ConfirmPhoneRequest request);
}

public class ProfileService(
    AppDbContext db,
    ICurrentUserService currentUser,
    UserManager<AppUser> userManager,
    IVerificationCodeSender codeSender) : IProfileService
{
    public async Task<ProfileDto> GetProfileAsync() =>
        await ToDtoAsync(await GetUserAsync());

    public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = await GetUserAsync();

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

    public async Task<SettingsDto> GetSettingsAsync()
    {
        var user = await GetUserAsync();
        return ToSettingsDto(user, await GetOrCreateSettingsAsync());
    }

    public async Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsRequest request)
    {
        var user = await GetUserAsync();
        var settings = await GetOrCreateSettingsAsync();

        if ((request.SmsEnabled || request.TwoFactorMethod == TwoFactorMethod.Sms) && !user.PhoneNumberConfirmed)
        {
            throw new BusinessRuleException(
                "Du skal bekræfte dit telefonnummer, før du kan bruge SMS.", "phoneNotConfirmed");
        }

        if (request.TwoFactorMethod == TwoFactorMethod.Email && !user.EmailConfirmed)
        {
            throw new BusinessRuleException(
                "Du skal bekræfte din e-mail, før den kan bruges til login-koder.", "emailNotConfirmed");
        }

        settings.EmailEnabled = request.EmailEnabled;
        settings.SmsEnabled = request.SmsEnabled;
        settings.TimeZone = request.TimeZone;
        user.TwoFactorMethod = request.TwoFactorMethod;

        await db.SaveChangesAsync();

        return ToSettingsDto(user, settings);
    }

    /// <summary>Sender en kode til det nye nummer. Nummeret gemmes først, når koden er bekræftet.</summary>
    public async Task RequestPhoneChangeAsync(ChangePhoneRequest request)
    {
        var user = await GetUserAsync();
        var phoneNumber = request.PhoneNumber.Trim();

        var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        await codeSender.SendAsync(user, TwoFactorMethod.Sms, CodePurpose.ConfirmPhone, code, phoneNumber);
    }

    public async Task<SettingsDto> ConfirmPhoneChangeAsync(ConfirmPhoneRequest request)
    {
        var user = await GetUserAsync();

        if (await userManager.IsLockedOutAsync(user))
        {
            throw new AccessDeniedException("For mange forkerte forsøg. Prøv igen om lidt.", "lockedOut");
        }

        // Gemmer nummeret og markerer det som bekræftet, hvis koden passer til netop dette nummer
        var result = await userManager.ChangePhoneNumberAsync(user, request.PhoneNumber.Trim(), request.Code);

        if (!result.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            throw new RequestValidationException(nameof(request.Code), "Koden er forkert eller udløbet.");
        }

        await userManager.ResetAccessFailedCountAsync(user);

        return ToSettingsDto(user, await GetOrCreateSettingsAsync());
    }

    private async Task<AppUser> GetUserAsync() =>
        await db.Users.SingleAsync(u => u.Id == currentUser.UserId);

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

    private static SettingsDto ToSettingsDto(AppUser user, UserSettings settings) => new(
        settings.EmailEnabled,
        settings.SmsEnabled,
        settings.TimeZone,
        user.TwoFactorMethod,
        user.EmailConfirmed,
        user.PhoneNumber,
        user.PhoneNumberConfirmed);

    private async Task<ProfileDto> ToDtoAsync(AppUser user)
    {
        var level = LevelCalculator.FromTotalXp(user.TotalXp);

        var completedTasks = await db.Tasks
            .CountAsync(t => t.UserId == user.Id && t.Status == TaskItemStatus.Completed);

        var inventoryCount = await db.UserInventory.CountAsync(i => i.UserId == user.Id);

        return new ProfileDto(
            user.Id,
            user.Email!,
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
