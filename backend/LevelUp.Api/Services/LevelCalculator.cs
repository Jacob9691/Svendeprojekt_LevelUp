namespace LevelUp.Api.Services;

public record LevelInfo(int Level, int XpIntoLevel, int XpForNextLevel);

/// <summary>
/// Beregner level ud fra samlet XP. Det kræver 100 × level XP at stige fra et level til det næste:
/// level 1 → 2 kræver 100 XP, level 2 → 3 kræver 200 XP osv.
/// </summary>
public static class LevelCalculator
{
    public const int XpPerLevelStep = 100;

    public static LevelInfo FromTotalXp(int totalXp)
    {
        var level = 1;
        var remaining = Math.Max(0, totalXp);

        while (remaining >= XpPerLevelStep * level)
        {
            remaining -= XpPerLevelStep * level;
            level++;
        }

        return new LevelInfo(level, remaining, XpPerLevelStep * level);
    }
}

/// <summary>Belønning for at gennemføre en opgave.</summary>
public static class Rewards
{
    public const int TaskXp = 20;
    public const int TaskPoints = 10;
}
