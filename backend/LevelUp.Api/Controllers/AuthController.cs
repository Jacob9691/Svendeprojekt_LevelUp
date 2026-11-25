using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LevelUp.Api.Controllers;

/// <summary>
/// Registrering og login. Alle endpoints er åbne, men begrænset til et antal kald pr. minut
/// for at gøre det svært at gætte adgangskoder og koder.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting(RateLimitPolicies.Auth)]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Opretter en konto og sender en bekræftelseskode via e-mail eller SMS.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request) =>
        await authService.RegisterAsync(request);

    /// <summary>Bekræfter kontoen med koden fra e-mail eller SMS.</summary>
    [HttpPost("confirm")]
    public async Task<ActionResult<MessageResponse>> Confirm(ConfirmAccountRequest request)
    {
        await authService.ConfirmAccountAsync(request);
        return new MessageResponse("Kontoen er bekræftet. Du kan nu logge ind.");
    }

    /// <summary>Sender en ny bekræftelseskode. Svarer altid OK, også hvis e-mailen ikke findes.</summary>
    [HttpPost("confirm/resend")]
    public async Task<ActionResult<MessageResponse>> ResendConfirmation(ResendConfirmationRequest request)
    {
        await authService.ResendConfirmationAsync(request);
        return new MessageResponse("Hvis kontoen findes og ikke er bekræftet, er der sendt en ny kode.");
    }

    /// <summary>Trin 1: tjekker e-mail og adgangskode og sender en engangskode (2FA).</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request) =>
        await authService.LoginAsync(request);

    /// <summary>Trin 2: tjekker engangskoden og returnerer et JWT access token.</summary>
    [HttpPost("login/2fa")]
    public async Task<ActionResult<AuthResponse>> VerifyTwoFactor(VerifyTwoFactorRequest request) =>
        await authService.VerifyTwoFactorAsync(request);

    /// <summary>Sender en ny engangskode til login.</summary>
    [HttpPost("login/2fa/resend")]
    public async Task<ActionResult<LoginResponse>> ResendTwoFactor(ResendTwoFactorRequest request) =>
        await authService.ResendTwoFactorAsync(request);
}

public static class RateLimitPolicies
{
    public const string Auth = "auth";
}
