using LevelUp.Api.Models;

namespace LevelUp.Api.Services.Messaging;

public enum CodePurpose
{
    ConfirmAccount,
    Login,
    ConfirmPhone,
}

public interface IVerificationCodeSender
{
    /// <summary>Sender en engangskode til brugeren via e-mail eller SMS.</summary>
    Task SendAsync(AppUser user, TwoFactorMethod channel, CodePurpose purpose, string code, string? phoneNumber = null);
}

/// <summary>Formulerer beskederne med engangskoder og sender dem via den rigtige kanal.</summary>
public class VerificationCodeSender(IEmailSender emailSender, ISmsSender smsSender) : IVerificationCodeSender
{
    public Task SendAsync(AppUser user, TwoFactorMethod channel, CodePurpose purpose, string code, string? phoneNumber = null)
    {
        var (subject, text) = purpose switch
        {
            CodePurpose.ConfirmAccount => (
                "Bekræft din LevelUp-konto",
                $"Velkommen til LevelUp, {user.DisplayName}! Din bekræftelseskode er {code}."),
            CodePurpose.Login => (
                "Din LevelUp-loginkode",
                $"Din loginkode til LevelUp er {code}. Har du ikke prøvet at logge ind, så skift din adgangskode."),
            CodePurpose.ConfirmPhone => (
                "Bekræft dit telefonnummer",
                $"Din kode til at bekræfte dit telefonnummer på LevelUp er {code}."),
            _ => throw new ArgumentOutOfRangeException(nameof(purpose)),
        };

        if (channel == TwoFactorMethod.Sms)
        {
            var to = phoneNumber ?? user.PhoneNumber
                ?? throw new InvalidOperationException("Brugeren har intet telefonnummer.");
            return smsSender.SendAsync(to, text);
        }

        var body = $"{text}\n\nKoden udløber om få minutter.\n\nHilsen LevelUp";
        return emailSender.SendAsync(user.Email!, subject, body);
    }
}
