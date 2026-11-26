namespace LevelUp.Api.Models;

/// <summary>Hvor ofte en opgave gentages – bruges af gentagende opgaver og udfordringer.</summary>
public enum RecurrenceFrequency
{
    Daily,
    /// <summary>Mandag til fredag.</summary>
    Weekdays,
    Weekly,
    Biweekly,
    Monthly,
}
