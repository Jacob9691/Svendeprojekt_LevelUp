using System.Security.Claims;
using System.Text;
using LevelUp.Api.Common;
using LevelUp.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LevelUp.Api.Services;

public record AccessToken(string Token, DateTime ExpiresAt);

public interface IJwtTokenService
{
    /// <summary>Token der giver adgang til API'et.</summary>
    AccessToken CreateAccessToken(AppUser user);

    /// <summary>Kortlivet token der kun beviser at adgangskoden var korrekt – bruges til 2FA-trinnet.</summary>
    string CreateTwoFactorToken(AppUser user);

    /// <summary>Returnerer bruger-id'et, hvis 2FA-tokenet er gyldigt.</summary>
    Task<int?> ValidateTwoFactorTokenAsync(string token);
}

public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _handler = new();

    /// <summary>
    /// 2FA-tokens får en anden audience end access tokens, så de ikke kan bruges til at kalde API'et.
    /// </summary>
    private string TwoFactorAudience => _options.Audience + ".2fa";

    public static SymmetricSecurityKey CreateSigningKey(string key) => new(Encoding.UTF8.GetBytes(key));

    public AccessToken CreateAccessToken(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var token = CreateToken(
            _options.Audience,
            expiresAt,
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName));

        return new AccessToken(token, expiresAt);
    }

    public string CreateTwoFactorToken(AppUser user) =>
        CreateToken(
            TwoFactorAudience,
            DateTime.UtcNow.AddMinutes(_options.TwoFactorTokenMinutes),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

    public async Task<int?> ValidateTwoFactorTokenAsync(string token)
    {
        var result = await _handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = _options.Issuer,
            ValidAudience = TwoFactorAudience,
            IssuerSigningKey = CreateSigningKey(_options.SigningKey),
            ClockSkew = TimeSpan.FromSeconds(30),
        });

        if (!result.IsValid
            || !result.Claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var sub)
            || !int.TryParse(sub?.ToString(), out var userId))
        {
            return null;
        }

        return userId;
    }

    private string CreateToken(string audience, DateTime expiresAt, params Claim[] claims)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = audience,
            Expires = expiresAt,
            Subject = new ClaimsIdentity(
                [.. claims, new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())]),
            SigningCredentials = new SigningCredentials(
                CreateSigningKey(_options.SigningKey), SecurityAlgorithms.HmacSha256),
        };

        return _handler.CreateToken(descriptor);
    }
}
