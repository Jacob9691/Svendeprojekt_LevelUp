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
    string TimeZone,
    TwoFactorMethod TwoFactorMethod,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed);

public class UpdateSettingsRequest : IValidatableObject
{
    public bool EmailEnabled { get; init; }
    public bool SmsEnabled { get; init; }

    [Required, MaxLength(50)]
    public string TimeZone { get; init; } = string.Empty;

    /// <summary>Hvordan loginkoden sendes. SMS kræver et bekræftet telefonnummer.</summary>
    public TwoFactorMethod TwoFactorMethod { get; init; } = TwoFactorMethod.Email;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(TimeZone) && !TimeZoneInfo.TryFindSystemTimeZoneById(TimeZone, out _))
        {
            yield return new ValidationResult(
                $"Ukendt tidszone '{TimeZone}'.",
                [nameof(TimeZone)]);
        }
    }
}

public class ChangePhoneRequest
{
    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; init; } = string.Empty;
}

public class ConfirmPhoneRequest
{
    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Koden består af 6 cifre.")]
    public string Code { get; init; } = string.Empty;
}
