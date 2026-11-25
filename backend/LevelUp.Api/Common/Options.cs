using System.ComponentModel.DataAnnotations;

namespace LevelUp.Api.Common;

/// <summary>Indstillinger for JWT. Sektionen "Jwt" i appsettings – SigningKey ligger i user-secrets.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>Hemmelig nøgle til at signere tokens. Mindst 32 tegn (256 bit).</summary>
    [Required, MinLength(32)]
    public string SigningKey { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 60;

    /// <summary>Hvor længe brugeren har til at indtaste 2FA-koden efter adgangskoden.</summary>
    [Range(1, 60)]
    public int TwoFactorTokenMinutes { get; init; } = 10;
}

/// <summary>Indstillinger for afsendelse af e-mails. Sektionen "Smtp" i appsettings.</summary>
public class SmtpOptions
{
    public const string SectionName = "Smtp";

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; } = 25;

    public bool EnableSsl { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }

    [Required, EmailAddress]
    public string FromAddress { get; init; } = string.Empty;

    public string FromName { get; init; } = "LevelUp";
}
