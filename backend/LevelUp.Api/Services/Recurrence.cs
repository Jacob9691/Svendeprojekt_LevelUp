using LevelUp.Api.Common;
using LevelUp.Api.Models;

namespace LevelUp.Api.Services;

/// <summary>Beregner starttidspunkterne for gentagende opgaver og udfordringer.</summary>
public static class Recurrence
{
    /// <summary>Højeste antal opgaver en serie må give – så man ikke ved en fejl laver tusindvis.</summary>
    public const int MaxOccurrences = 200;

    /// <summary>
    /// Alle starttidspunkter fra start til og med until.
    /// </summary>
    public static List<DateTime> Until(DateTime start, RecurrenceFrequency frequency, DateOnly until, string errorField)
    {
        var lastAllowed = until.ToDateTime(TimeOnly.MaxValue);
        var result = Generate(start, frequency).TakeWhile(next => next <= lastAllowed).Take(MaxOccurrences + 1).ToList();

        if (result.Count > MaxOccurrences)
        {
            throw new RequestValidationException(errorField,
                $"En serie må højst have {MaxOccurrences} gentagelser. Vælg en tidligere slutdato.");
        }

        if (result.Count == 0)
        {
            throw new RequestValidationException(errorField,
                "Der er ingen dage i perioden der passer til gentagelsen.");
        }

        return result;
    }

    /// <summary>De første <paramref name="count"/> starttidspunkter – bruges af udfordringer.</summary>
    public static List<DateTime> Count(DateTime start, RecurrenceFrequency frequency, int count) =>
        Generate(start, frequency).Take(Math.Min(count, MaxOccurrences)).ToList();

    /// <summary>
    /// Uendelig række af starttidspunkter. Regnes altid ud fra den første dato (f.eks. start.AddMonths(i)),
    /// så en månedlig opgave d. 31. ikke glider til d. 28. Hverdage springer lørdag og søndag over.
    /// </summary>
    private static IEnumerable<DateTime> Generate(DateTime start, RecurrenceFrequency frequency)
    {
        for (var i = 0; ; i++)
        {
            var next = frequency switch
            {
                RecurrenceFrequency.Daily or RecurrenceFrequency.Weekdays => start.AddDays(i),
                RecurrenceFrequency.Weekly => start.AddDays(7 * i),
                RecurrenceFrequency.Biweekly => start.AddDays(14 * i),
                RecurrenceFrequency.Monthly => start.AddMonths(i),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency)),
            };

            if (frequency == RecurrenceFrequency.Weekdays && next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            yield return next;
        }
    }
}
