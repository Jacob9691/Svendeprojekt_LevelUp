using LevelUp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Data;

public static class SeedData
{
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
    /// Opretter en demo-bruger i Development, så API'et kan testes før login er implementeret.
    /// Fjernes når login og JWT er på plads.
    /// </summary>
    public static async Task SeedDevelopmentDataAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var user = new AppUser
        {
            Email = "demo@levelup.local",
            DisplayName = "Demo",
            PasswordHash = string.Empty,
            CoinBalance = 100,
            Settings = new UserSettings(),
            Character = new UserCharacter { CharacterName = "Demo-helten" },
        };
        user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, "Demo1234!");

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
