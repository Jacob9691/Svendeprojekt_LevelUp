using LevelUp.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LevelUp.Api.Data;

public static class SeedData
{
    public const string DemoEmail = "demo@levelup.local";

    /// <summary>Genstande i shoppen. Lægges ind via migrationen (HasData).</summary>
    public static readonly ShopItem[] ShopItems =
    [
        new() { Id = 1, Name = "Ridderhjelm", Description = "En skinnende hjelm til modige planlæggere.", Price = 50, Category = "Hat" },
        new() { Id = 2, Name = "Troldmandshat", Description = "Spids hat med stjerner.", Price = 80, Category = "Hat" },
        new() { Id = 3, Name = "Rød kappe", Description = "Flagrer flot i vinden.", Price = 60, Category = "Outfit" },
        new() { Id = 4, Name = "Ninjadragt", Description = "Til dem der løser opgaver i stilhed.", Price = 120, Category = "Outfit" },
        new() { Id = 5, Name = "Træsværd", Description = "Starterudstyr for eventyrere.", Price = 30, Category = "Accessory" },
        new() { Id = 6, Name = "Gyldent skjold", Description = "Kun for de mest produktive.", Price = 200, Category = "Accessory" },
    ];

    /// <summary>
    /// Opretter en bekræftet demo-bruger i Development, så API'et hurtigt kan afprøves.
    /// Adgangskoden læses fra konfigurationen "DemoUser:Password" (user-secrets).
    /// </summary>
    public static async Task SeedDevelopmentDataAsync(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        var password = configuration["DemoUser:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var existing = await userManager.FindByEmailAsync(DemoEmail);

        if (existing is not null)
        {
            // Sørg for at demo-brugeren altid har adgangskoden fra user-secrets
            if (!await userManager.CheckPasswordAsync(existing, password))
            {
                await userManager.RemovePasswordAsync(existing);
                await userManager.AddPasswordAsync(existing, password);
            }

            return;
        }

        var user = new AppUser
        {
            UserName = DemoEmail,
            Email = DemoEmail,
            EmailConfirmed = true,
            TwoFactorEnabled = true,
            DisplayName = "Demo",
            CoinBalance = 100,
            Settings = new UserSettings(),
            Character = new UserCharacter { CharacterName = "Demo-helten" },
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Kunne ikke oprette demo-bruger: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
