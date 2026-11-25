namespace LevelUp.Api.Models;

/// <summary>Hvornår brugeren har tid i løbet af ugen – sættes under onboarding.</summary>
public class WeeklyAvailability
{
    public int Id { get; set; }
    public int UserId { get; set; }

    /// <summary>Gemmes som tal: 0 = søndag, 1 = mandag … 6 = lørdag.</summary>
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public AppUser? User { get; set; }
}
