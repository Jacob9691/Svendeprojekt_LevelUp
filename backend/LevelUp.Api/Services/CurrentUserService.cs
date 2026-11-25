namespace LevelUp.Api.Services;

/// <summary>Giver id'et på den bruger, der laver requesten.</summary>
public interface ICurrentUserService
{
    int UserId { get; }
}

/// <summary>
/// Midlertidig implementering der altid returnerer demo-brugeren (id 1).
/// Erstattes af en implementering der læser bruger-id'et fra JWT-tokenet.
/// </summary>
public class DevCurrentUserService : ICurrentUserService
{
    public int UserId => 1;
}
