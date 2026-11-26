using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;
using static LevelUp.Api.Dtos.ValidationMessages;

namespace LevelUp.Api.Dtos;

public class RegisterRequest : IValidatableObject
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Required), EmailAddress(ErrorMessage = ValidationMessages.Email), MaxLength(255, ErrorMessage = MaxLength)]
    public string Email { get; init; } = string.Empty;

    [Display(Name = "Adgangskoden")]
    [Required(ErrorMessage = Required), MinLength(8, ErrorMessage = MinLength), MaxLength(100, ErrorMessage = MaxLength)]
    public string Password { get; init; } = string.Empty;

    [Display(Name = "Navn")]
    [Required(ErrorMessage = Required), MaxLength(100, ErrorMessage = MaxLength)]
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Krævet hvis kontoen skal bekræftes via SMS. Format: +4512345678.</summary>
    [Display(Name = "Telefonnummeret")]
    [Phone(ErrorMessage = Phone), MaxLength(20, ErrorMessage = MaxLength)]
    public string? PhoneNumber { get; init; }

    /// <summary>Hvor bekræftelseskoden skal sendes hen. Bliver også brugerens 2FA-metode.</summary>
    public TwoFactorMethod ConfirmationMethod { get; init; } = TwoFactorMethod.Email;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ConfirmationMethod == TwoFactorMethod.Sms && string.IsNullOrWhiteSpace(PhoneNumber))
        {
            yield return new ValidationResult(
                "Der skal angives et telefonnummer for at bekræfte via SMS.",
                [nameof(PhoneNumber)]);
        }
    }
}

public record RegisterResponse(string Message, TwoFactorMethod ConfirmationMethod, string SentTo);

public class ConfirmAccountRequest
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Required), EmailAddress(ErrorMessage = ValidationMessages.Email)]
    public string Email { get; init; } = string.Empty;

    [Display(Name = "Koden")]
    [Required(ErrorMessage = Required), RegularExpression(@"^\d{6}$", ErrorMessage = ValidationMessages.Code)]
    public string Code { get; init; } = string.Empty;

    public TwoFactorMethod Method { get; init; } = TwoFactorMethod.Email;
}

public class ResendConfirmationRequest
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Required), EmailAddress(ErrorMessage = ValidationMessages.Email)]
    public string Email { get; init; } = string.Empty;

    public TwoFactorMethod Method { get; init; } = TwoFactorMethod.Email;
}

public class LoginRequest
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = Required), EmailAddress(ErrorMessage = ValidationMessages.Email)]
    public string Email { get; init; } = string.Empty;

    [Display(Name = "Adgangskoden")]
    [Required(ErrorMessage = Required)]
    public string Password { get; init; } = string.Empty;
}

/// <summary>Svar på trin 1 af login. Koden er sendt, og twoFactorToken skal bruges i trin 2.</summary>
public record LoginResponse(string TwoFactorToken, TwoFactorMethod Method, string SentTo);

public class VerifyTwoFactorRequest
{
    [Required(ErrorMessage = TwoFactorTokenMissing)]
    public string TwoFactorToken { get; init; } = string.Empty;

    [Display(Name = "Koden")]
    [Required(ErrorMessage = Required), RegularExpression(@"^\d{6}$", ErrorMessage = ValidationMessages.Code)]
    public string Code { get; init; } = string.Empty;
}

public class ResendTwoFactorRequest
{
    [Required(ErrorMessage = TwoFactorTokenMissing)]
    public string TwoFactorToken { get; init; } = string.Empty;
}

public record AuthUserDto(int Id, string Email, string DisplayName);

/// <summary>Svar efter gennemført login. accessToken sendes som "Authorization: Bearer ..." fremover.</summary>
public record AuthResponse(string AccessToken, DateTime ExpiresAt, AuthUserDto User);

public record MessageResponse(string Message);
