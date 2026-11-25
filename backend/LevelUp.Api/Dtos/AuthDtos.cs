using System.ComponentModel.DataAnnotations;
using LevelUp.Api.Models;

namespace LevelUp.Api.Dtos;

public class RegisterRequest : IValidatableObject
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Krævet hvis kontoen skal bekræftes via SMS. Format: +4512345678.</summary>
    [Phone, MaxLength(20)]
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
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Koden består af 6 cifre.")]
    public string Code { get; init; } = string.Empty;

    public TwoFactorMethod Method { get; init; } = TwoFactorMethod.Email;
}

public class ResendConfirmationRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    public TwoFactorMethod Method { get; init; } = TwoFactorMethod.Email;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

/// <summary>Svar på trin 1 af login. Koden er sendt, og twoFactorToken skal bruges i trin 2.</summary>
public record LoginResponse(string TwoFactorToken, TwoFactorMethod Method, string SentTo);

public class VerifyTwoFactorRequest
{
    [Required]
    public string TwoFactorToken { get; init; } = string.Empty;

    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Koden består af 6 cifre.")]
    public string Code { get; init; } = string.Empty;
}

public class ResendTwoFactorRequest
{
    [Required]
    public string TwoFactorToken { get; init; } = string.Empty;
}

public record AuthUserDto(int Id, string Email, string DisplayName);

/// <summary>Svar efter gennemført login. accessToken sendes som "Authorization: Bearer ..." fremover.</summary>
public record AuthResponse(string AccessToken, DateTime ExpiresAt, AuthUserDto User);

public record MessageResponse(string Message);
