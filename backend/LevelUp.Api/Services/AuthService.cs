using LevelUp.Api.Common;
using LevelUp.Api.Dtos;
using LevelUp.Api.Models;
using LevelUp.Api.Services.Messaging;
using Microsoft.AspNetCore.Identity;

namespace LevelUp.Api.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task ConfirmAccountAsync(ConfirmAccountRequest request);
    Task ResendConfirmationAsync(ResendConfirmationRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> VerifyTwoFactorAsync(VerifyTwoFactorRequest request);
    Task<LoginResponse> ResendTwoFactorAsync(ResendTwoFactorRequest request);
}

/// <summary>
/// Registrering, kontobekræftelse og login i to trin:
/// 1) e-mail + adgangskode → engangskode sendes, og der returneres et kortlivet 2FA-token
/// 2) 2FA-token + engangskode → JWT access token
/// </summary>
public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtTokenService jwtTokenService,
    IVerificationCodeSender codeSender,
    ILogger<AuthService> logger) : IAuthService
{
    private const string InvalidCodeMessage = "Koden er forkert eller udløbet.";
    private const string InvalidLoginMessage = "Forkert e-mail eller adgangskode.";

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();

        var user = new AppUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = phoneNumber,
            DisplayName = request.DisplayName.Trim(),
            TwoFactorEnabled = true,
            TwoFactorMethod = request.ConfirmationMethod,
            Settings = new UserSettings(),
            Character = new UserCharacter(),
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw ToValidationException(result);
        }

        logger.LogInformation("Ny bruger {UserId} registreret", user.Id);

        await SendConfirmationCodeAsync(user, request.ConfirmationMethod);

        return new RegisterResponse(
            "Kontoen er oprettet. Indtast koden vi har sendt for at bekræfte den.",
            request.ConfirmationMethod,
            Mask(user, request.ConfirmationMethod));
    }

    public async Task ConfirmAccountAsync(ConfirmAccountRequest request)
    {
        // Samme fejlbesked uanset om brugeren findes – så man ikke kan gætte hvilke e-mails der er oprettet
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new RequestValidationException(nameof(request.Code), InvalidCodeMessage);

        await EnsureNotLockedOutAsync(user);

        var result = request.Method == TwoFactorMethod.Sms
            ? user.PhoneNumber is null
                ? IdentityResult.Failed()
                : await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, request.Code)
            : await userManager.ConfirmEmailAsync(user, request.Code);

        if (!result.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            throw new RequestValidationException(nameof(request.Code), InvalidCodeMessage);
        }

        await userManager.ResetAccessFailedCountAsync(user);
        logger.LogInformation("Bruger {UserId} bekræftede {Method}", user.Id, request.Method);
    }

    public async Task ResendConfirmationAsync(ResendConfirmationRequest request)
    {
        // Svarer altid OK, så man ikke kan bruge endpointet til at finde ud af hvilke e-mails der findes
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return;
        }

        var alreadyConfirmed = request.Method == TwoFactorMethod.Sms
            ? user.PhoneNumberConfirmed || user.PhoneNumber is null
            : user.EmailConfirmed;

        if (!alreadyConfirmed)
        {
            await SendConfirmationCodeAsync(user, request.Method);
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new AuthenticationFailedException(InvalidLoginMessage);

        // Tjekker lockout, om kontoen er bekræftet, og adgangskoden. Tæller forkerte forsøg op.
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            throw LockedOut();
        }

        if (result.IsNotAllowed)
        {
            // Afslør kun at kontoen mangler bekræftelse, hvis adgangskoden er rigtig
            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new AuthenticationFailedException(InvalidLoginMessage);
            }

            throw new AccessDeniedException(
                "Kontoen er ikke bekræftet endnu. Indtast koden vi har sendt, eller bed om en ny.",
                "accountNotConfirmed");
        }

        if (!result.Succeeded)
        {
            throw new AuthenticationFailedException(InvalidLoginMessage);
        }

        return await SendLoginCodeAsync(user);
    }

    public async Task<AuthResponse> VerifyTwoFactorAsync(VerifyTwoFactorRequest request)
    {
        var user = await GetUserFromTwoFactorTokenAsync(request.TwoFactorToken);

        await EnsureNotLockedOutAsync(user);

        var provider = ToTokenProvider(ResolveTwoFactorMethod(user));
        var valid = await userManager.VerifyTwoFactorTokenAsync(user, provider, request.Code);

        if (!valid)
        {
            await userManager.AccessFailedAsync(user);
            throw new AuthenticationFailedException(InvalidCodeMessage, "invalidCode");
        }

        await userManager.ResetAccessFailedCountAsync(user);
        logger.LogInformation("Bruger {UserId} loggede ind", user.Id);

        var accessToken = jwtTokenService.CreateAccessToken(user);
        return new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            new AuthUserDto(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<LoginResponse> ResendTwoFactorAsync(ResendTwoFactorRequest request)
    {
        var user = await GetUserFromTwoFactorTokenAsync(request.TwoFactorToken);

        await EnsureNotLockedOutAsync(user);

        return await SendLoginCodeAsync(user);
    }

    private async Task<LoginResponse> SendLoginCodeAsync(AppUser user)
    {
        var method = ResolveTwoFactorMethod(user);
        var code = await userManager.GenerateTwoFactorTokenAsync(user, ToTokenProvider(method));

        await codeSender.SendAsync(user, method, CodePurpose.Login, code);

        return new LoginResponse(jwtTokenService.CreateTwoFactorToken(user), method, Mask(user, method));
    }

    private async Task SendConfirmationCodeAsync(AppUser user, TwoFactorMethod method)
    {
        var code = method == TwoFactorMethod.Sms
            ? await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!)
            : await userManager.GenerateEmailConfirmationTokenAsync(user);

        await codeSender.SendAsync(user, method, CodePurpose.ConfirmAccount, code);
    }

    private async Task<AppUser> GetUserFromTwoFactorTokenAsync(string twoFactorToken)
    {
        var userId = await jwtTokenService.ValidateTwoFactorTokenAsync(twoFactorToken)
            ?? throw new AuthenticationFailedException(
                "Login-sessionen er udløbet. Log ind med e-mail og adgangskode igen.", "twoFactorTokenExpired");

        return await userManager.FindByIdAsync(userId.ToString())
            ?? throw new AuthenticationFailedException(InvalidLoginMessage);
    }

    private async Task EnsureNotLockedOutAsync(AppUser user)
    {
        if (await userManager.IsLockedOutAsync(user))
        {
            throw LockedOut();
        }
    }

    /// <summary>
    /// Bruger den valgte 2FA-metode, men falder tilbage til den anden kanal,
    /// hvis den valgte ikke er bekræftet (f.eks. SMS valgt men telefonnummer ikke bekræftet).
    /// </summary>
    private static TwoFactorMethod ResolveTwoFactorMethod(AppUser user) =>
        user.TwoFactorMethod switch
        {
            TwoFactorMethod.Sms when user.PhoneNumberConfirmed => TwoFactorMethod.Sms,
            TwoFactorMethod.Email when !user.EmailConfirmed && user.PhoneNumberConfirmed => TwoFactorMethod.Sms,
            _ => TwoFactorMethod.Email,
        };

    private static string ToTokenProvider(TwoFactorMethod method) =>
        method == TwoFactorMethod.Sms ? TokenOptions.DefaultPhoneProvider : TokenOptions.DefaultEmailProvider;

    private static AccessDeniedException LockedOut() => new(
        "For mange forkerte forsøg. Kontoen er låst i et par minutter.", "lockedOut");

    /// <summary>Viser kun en del af e-mail/telefonnummer, f.eks. "j***@hotmail.com" eller "****5678".</summary>
    private static string Mask(AppUser user, TwoFactorMethod method)
    {
        if (method == TwoFactorMethod.Sms)
        {
            var phone = user.PhoneNumber ?? string.Empty;
            return phone.Length <= 4 ? "****" : "****" + phone[^4..];
        }

        var email = user.Email ?? string.Empty;
        var at = email.IndexOf('@');
        return at <= 1 ? "***" + email[Math.Max(at, 0)..] : email[0] + "***" + email[at..];
    }

    private static RequestValidationException ToValidationException(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.Code switch
            {
                var c when c.StartsWith("Password") => "Password",
                var c when c.Contains("Email") || c.Contains("UserName") => "Email",
                _ => string.Empty,
            })
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).Distinct().ToArray());

        return new RequestValidationException(errors);
    }
}
