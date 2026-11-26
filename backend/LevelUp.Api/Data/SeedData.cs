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

    /// <summary>Faste mål brugeren kan nå. Lægges ind via migrationen (HasData).</summary>
    public static readonly Achievement[] Achievements =
    [
        new() { Id = 1, Name = "Første skridt", Description = "Gennemfør din første opgave.", Type = AchievementType.TasksCompleted, RequiredCount = 1 },
        new() { Id = 2, Name = "Godt i gang", Description = "Gennemfør 10 opgaver.", Type = AchievementType.TasksCompleted, RequiredCount = 10 },
        new() { Id = 3, Name = "Opgavemester", Description = "Gennemfør 50 opgaver.", Type = AchievementType.TasksCompleted, RequiredCount = 50 },
        new() { Id = 4, Name = "Level 5", Description = "Nå level 5.", Type = AchievementType.Level, RequiredCount = 5 },
        new() { Id = 5, Name = "Level 10", Description = "Nå level 10.", Type = AchievementType.Level, RequiredCount = 10 },
        new() { Id = 6, Name = "Level 20", Description = "Nå level 20.", Type = AchievementType.Level, RequiredCount = 20 },
        new() { Id = 7, Name = "Første køb", Description = "Køb din første genstand i shoppen.", Type = AchievementType.ItemsBought, RequiredCount = 1 },
        new() { Id = 8, Name = "Samler", Description = "Køb 5 genstande.", Type = AchievementType.ItemsBought, RequiredCount = 5 },
        new() { Id = 9, Name = "Udfordrer", Description = "Gennemfør din første udfordring.", Type = AchievementType.ChallengesCompleted, RequiredCount = 1 },
        new() { Id = 10, Name = "Udholdende", Description = "Gennemfør 3 udfordringer.", Type = AchievementType.ChallengesCompleted, RequiredCount = 3 },
    ];

    /// <summary>Udfordringer brugeren kan sætte i sin kalender. Lægges ind via migrationen (HasData).</summary>
    public static readonly Challenge[] Challenges =
    [
        new() { Id = 1, Name = "7 dages gåtur", Description = "Gå en tur hver dag i en uge.", TaskTitle = "Gå en tur", Repeat = RecurrenceFrequency.Daily, RequiredCount = 7, RewardXp = 100, RewardCoins = 50 },
        new() { Id = 2, Name = "Læseuge", Description = "Læs 20 minutter hver hverdag i to uger.", TaskTitle = "Læs 20 minutter", Repeat = RecurrenceFrequency.Weekdays, RequiredCount = 10, RewardXp = 150, RewardCoins = 75 },
        new() { Id = 3, Name = "Rent hjem", Description = "Gør rent én gang om ugen i en måned.", TaskTitle = "Gør rent", Repeat = RecurrenceFrequency.Weekly, RequiredCount = 4, RewardXp = 120, RewardCoins = 60 },
        new() { Id = 4, Name = "Morgenrutine", Description = "Lav din morgenrutine hver dag i 14 dage.", TaskTitle = "Morgenrutine", Repeat = RecurrenceFrequency.Daily, RequiredCount = 14, RewardXp = 250, RewardCoins = 120 },
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
