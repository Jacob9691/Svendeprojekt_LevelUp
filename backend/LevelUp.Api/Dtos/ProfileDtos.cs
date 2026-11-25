using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;

namespace LevelUp.Api.Dtos;

public record ProfileDto(
    int Id,
    string Email,
    string DisplayName,
    int Level,
    int TotalXp,
    int XpIntoLevel,
    int XpForNextLevel,
    int CoinBalance,
    int CompletedTasks,
    int InventoryCount,
    DateTime CreatedAt);

public class UpdateProfileRequest
{
    [Required, MaxLength(100)]
    public string DisplayName { get; init; } = string.Empty;
}

public record PointTransactionDto(
    int Id,
    int Amount,
    PointTransactionType Type,
    int? TaskId,
    int? ShopItemId,
    DateTime CreatedAt);

public record SettingsDto(
    bool EmailEnabled,
    bool SmsEnabled,
    string? PhoneNumber,
    string TimeZone)
{
    public static SettingsDto FromEntity(UserSettings settings) => new(
        settings.EmailEnabled,
        settings.SmsEnabled,
        settings.PhoneNumber,
        settings.TimeZone);
}

public class UpdateSettingsRequest : IValidatableObject
{
    public bool EmailEnabled { get; init; }
    public bool SmsEnabled { get; init; }

    [Phone, MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [Required, MaxLength(50)]
    public string TimeZone { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SmsEnabled && string.IsNullOrWhiteSpace(PhoneNumber))
        {
            yield return new ValidationResult(
                "Der skal angives et telefonnummer for at aktivere SMS.",
                [nameof(PhoneNumber)]);
        }

        if (!string.IsNullOrWhiteSpace(TimeZone) && !TimeZoneInfo.TryFindSystemTimeZoneById(TimeZone, out _))
        {
            yield return new ValidationResult(
                $"Ukendt tidszone '{TimeZone}'.",
                [nameof(TimeZone)]);
        }
    }
}
