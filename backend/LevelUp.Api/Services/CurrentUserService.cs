using Microsoft.IdentityModel.JsonWebTokens;

namespace LevelUp.Api.Services;

/// <summary>Giver id'et på den bruger, der laver requesten.</summary>
public interface ICurrentUserService
{
    int UserId { get; }
}

/// <summary>
/// Læser bruger-id'et fra "sub"-claimet i JWT-tokenet. Alle services filtrerer på dette id,
/// så en bruger kun kan se og ændre sine egne data.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(sub, out var userId)
                ? userId
                : throw new InvalidOperationException("Requesten har ingen logget ind bruger.");
        }
    }
}
