using System.Net;
using System.Net.Mail;
using LevelUp.Api.Common;
using Microsoft.Extensions.Options;

namespace LevelUp.Api.Services.Messaging;

public interface IEmailSender
{
    Task SendAsync(string toAddress, string subject, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// Sender e-mails via SMTP. Under udvikling er det Mailpit i Docker, der fanger mailene
/// (se http://localhost:8025). I produktion peges den på en rigtig SMTP-server.
/// </summary>
public class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendAsync(string toAddress, string subject, string body, CancellationToken cancellationToken = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };
        message.To.Add(toAddress);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
        };

        if (!string.IsNullOrEmpty(_options.UserName))
        {
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
        logger.LogInformation("E-mail '{Subject}' sendt", subject);
    }
}
