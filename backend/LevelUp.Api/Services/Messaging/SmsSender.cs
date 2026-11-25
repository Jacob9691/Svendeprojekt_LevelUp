namespace LevelUp.Api.Services.Messaging;

public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Midlertidig SMS-afsender der skriver beskeden i loggen i stedet for at sende den.
/// Erstattes af en rigtig SMS-udbyder (f.eks. Twilio), hvis SMS skal virke i produktion.
/// </summary>
public class LoggingSmsSender(ILogger<LoggingSmsSender> logger) : ISmsSender
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("[SMS til {PhoneNumber}] {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}
