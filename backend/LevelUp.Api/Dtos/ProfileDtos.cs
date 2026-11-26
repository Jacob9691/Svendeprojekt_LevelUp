using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;
using static LevelUp.Api.Dtos.ValidationMessages;

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
    [Display(Name = "Navn")]
    [Required(ErrorMessage = Required), MaxLength(100, ErrorMessage = MaxLength)]
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

    [Display(Name = "Tidszone")]
    [Required(ErrorMessage = Required), MaxLength(50, ErrorMessage = MaxLength)]
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
    [Display(Name = "Telefonnummeret")]
    [Required(ErrorMessage = Required), Phone(ErrorMessage = Phone), MaxLength(20, ErrorMessage = MaxLength)]
    public string PhoneNumber { get; init; } = string.Empty;
}

public class ConfirmPhoneRequest
{
    [Display(Name = "Telefonnummeret")]
    [Required(ErrorMessage = Required), Phone(ErrorMessage = Phone), MaxLength(20, ErrorMessage = MaxLength)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Display(Name = "Koden")]
    [Required(ErrorMessage = Required), RegularExpression(@"^\d{6}$", ErrorMessage = ValidationMessages.Code)]
    public string Code { get; init; } = string.Empty;
}
