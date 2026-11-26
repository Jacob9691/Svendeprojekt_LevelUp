using System.Text.RegularExpressions;
using LevelUp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Api.Data;

/// <summary>
/// IdentityUserContext giver tabellerne til brugere, claims, eksterne logins og tokens.
/// Roller bruges ikke, så IdentityDbContext er ikke nødvendig.
/// </summary>
public partial class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityUserContext<AppUser, int>(options)
{
    private const string UtcNowSql = "SYSUTCDATETIME()";

    // Users kommer fra IdentityUserContext
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<UserCharacter> UserCharacters => Set<UserCharacter>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<WeeklyAvailability> WeeklyAvailability => Set<WeeklyAvailability>();
    public DbSet<PointTransaction> PointTransactions => Set<PointTransaction>();
    public DbSet<ShopItem> ShopItems => Set<ShopItem>();
    public DbSet<UserInventoryItem> UserInventory => Set<UserInventoryItem>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<UserChallenge> UserChallenges => Set<UserChallenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Opsætter Identity-tabellerne – skal kaldes først
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(user =>
        {
            user.Property(u => u.Email).HasMaxLength(255);
            user.Property(u => u.PasswordHash).HasMaxLength(255);
            user.Property(u => u.PhoneNumber).HasMaxLength(20);
            user.Property(u => u.DisplayName).HasMaxLength(100);
            user.Property(u => u.TwoFactorMethod).HasConversion<string>().HasMaxLength(20);
            user.Property(u => u.TotalXp).HasDefaultValue(0);
            user.Property(u => u.CoinBalance).HasDefaultValue(0);
            user.Property(u => u.CreatedAt).HasDefaultValueSql(UtcNowSql);
            user.HasIndex(u => u.Email).IsUnique();

            // 1-til-1 relationer (giver automatisk et unikt index på user_id)
            user.HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId);

            user.HasOne(u => u.Character)
                .WithOne(c => c.User)
                .HasForeignKey<UserCharacter>(c => c.UserId);
        });

        modelBuilder.Entity<UserSettings>(settings =>
        {
            settings.Property(s => s.TimeZone).HasMaxLength(50);
        });

        modelBuilder.Entity<UserCharacter>(character =>
        {
            character.Property(c => c.CharacterName).HasMaxLength(100);
        });

        modelBuilder.Entity<TaskItem>(task =>
        {
            task.Property(t => t.Title).HasMaxLength(150);
            task.Property(t => t.Status).HasConversion<string>().HasMaxLength(30);
            task.Property(t => t.CreatedAt).HasDefaultValueSql(UtcNowSql);

            task.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId);

            task.HasIndex(t => new { t.UserId, t.ScheduledStart });
            task.HasIndex(t => new { t.UserId, t.SeriesId });
        });

        modelBuilder.Entity<WeeklyAvailability>(availability =>
        {
            availability.HasOne(a => a.User)
                .WithMany(u => u.WeeklyAvailability)
                .HasForeignKey(a => a.UserId);
        });

        modelBuilder.Entity<PointTransaction>(transaction =>
        {
            transaction.Property(t => t.Type).HasConversion<string>().HasMaxLength(30);
            transaction.Property(t => t.CreatedAt).HasDefaultValueSql(UtcNowSql);

            transaction.HasOne(t => t.User)
                .WithMany(u => u.PointTransactions)
                .HasForeignKey(t => t.UserId);

            // SQL Server tillader ikke to cascade-veje fra app_user til samme tabel
            // (app_user → point_transaction og app_user → task → point_transaction).
            // Derfor NO ACTION her – TaskService nulstiller referencen før en opgave slettes.
            transaction.HasOne(t => t.Task)
                .WithMany()
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            transaction.HasOne(t => t.ShopItem)
                .WithMany()
                .HasForeignKey(t => t.ShopItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ShopItem>(item =>
        {
            item.Property(i => i.Name).HasMaxLength(100);
            item.Property(i => i.Category).HasMaxLength(50);
            item.HasData(SeedData.ShopItems);
        });

        modelBuilder.Entity<UserInventoryItem>(inventory =>
        {
            inventory.Property(i => i.PurchasedAt).HasDefaultValueSql(UtcNowSql);

            inventory.HasOne(i => i.User)
                .WithMany(u => u.Inventory)
                .HasForeignKey(i => i.UserId);

            // En bruger kan kun eje den samme genstand én gang
            inventory.HasIndex(i => new { i.UserId, i.ShopItemId }).IsUnique();
        });

        modelBuilder.Entity<Reminder>(reminder =>
        {
            reminder.Property(r => r.Channel).HasConversion<string>().HasMaxLength(20);
            reminder.Property(r => r.Status).HasConversion<string>().HasMaxLength(30);

            reminder.HasOne(r => r.User)
                .WithMany(u => u.Reminders)
                .HasForeignKey(r => r.UserId);

            // Samme grund som ved point_transaction – TaskService sletter påmindelserne selv
            reminder.HasOne(r => r.Task)
                .WithMany()
                .HasForeignKey(r => r.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            reminder.HasIndex(r => new { r.Status, r.ScheduledAt });
        });

        modelBuilder.Entity<Achievement>(achievement =>
        {
            achievement.Property(a => a.Name).HasMaxLength(100);
            achievement.Property(a => a.Type).HasConversion<string>().HasMaxLength(30);
            achievement.HasData(SeedData.Achievements);
        });

        modelBuilder.Entity<UserAchievement>(userAchievement =>
        {
            userAchievement.Property(a => a.UnlockedAt).HasDefaultValueSql(UtcNowSql);

            userAchievement.HasOne(a => a.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(a => a.UserId);

            // Et achievement kan kun låses op én gang pr. bruger
            userAchievement.HasIndex(a => new { a.UserId, a.AchievementId }).IsUnique();
        });

        modelBuilder.Entity<Challenge>(challenge =>
        {
            challenge.Property(c => c.Name).HasMaxLength(100);
            challenge.Property(c => c.TaskTitle).HasMaxLength(150);
            challenge.Property(c => c.Repeat).HasConversion<string>().HasMaxLength(20);
            challenge.Property(c => c.RewardCoins).HasDefaultValue(0);
            challenge.Property(c => c.RewardXp).HasDefaultValue(0);
            challenge.HasData(SeedData.Challenges);
        });

        modelBuilder.Entity<UserChallenge>(userChallenge =>
        {
            userChallenge.Property(c => c.Progress).HasDefaultValue(0);
            userChallenge.Property(c => c.StartedAt).HasDefaultValueSql(UtcNowSql);

            userChallenge.HasOne(c => c.User)
                .WithMany(u => u.Challenges)
                .HasForeignKey(c => c.UserId);

            // En udfordring kan kun tages én gang pr. bruger
            userChallenge.HasIndex(c => new { c.UserId, c.ChallengeId }).IsUnique();
            userChallenge.HasIndex(c => new { c.UserId, c.SeriesId });
        });

        ApplySnakeCaseNames(modelBuilder);

        // Klasser hvis navn ikke matcher tabellen i ERD'en
        modelBuilder.Entity<TaskItem>().ToTable("task");
        modelBuilder.Entity<UserInventoryItem>().ToTable("user_inventory");

        // Identity's hjælpetabeller
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("user_claim");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("user_login");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("user_token");
    }

    /// <summary>
    /// Navngiver tabeller og kolonner i snake_case som i ERD'en (AppUser → app_user, TotalXp → total_xp).
    /// </summary>
    private static void ApplySnakeCaseNames(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.ClrType.Name));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string name) =>
        PascalCaseBoundary().Replace(name, "$1_$2").ToLowerInvariant();

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex PascalCaseBoundary();
}
