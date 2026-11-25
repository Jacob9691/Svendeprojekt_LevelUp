using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LevelUp.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    /// <summary>Brugerens oplysninger, level, XP og pointbalance.</summary>
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfile() =>
        await profileService.GetProfileAsync();

    [HttpPut]
    public async Task<ActionResult<ProfileDto>> UpdateProfile(UpdateProfileRequest request) =>
        await profileService.UpdateProfileAsync(request);

    /// <summary>Historik over optjente og brugte points.</summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<List<PointTransactionDto>>> GetTransactions() =>
        await profileService.GetTransactionsAsync();

    [HttpGet("settings")]
    public async Task<ActionResult<SettingsDto>> GetSettings() =>
        await profileService.GetSettingsAsync();

    /// <summary>Påmindelser, tidszone og 2FA-metode. SMS kræver et bekræftet telefonnummer.</summary>
    [HttpPut("settings")]
    public async Task<ActionResult<SettingsDto>> UpdateSettings(UpdateSettingsRequest request) =>
        await profileService.UpdateSettingsAsync(request);

    /// <summary>Sender en SMS-kode til et nyt telefonnummer.</summary>
    [HttpPost("phone")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<ActionResult<MessageResponse>> RequestPhoneChange(ChangePhoneRequest request)
    {
        await profileService.RequestPhoneChangeAsync(request);
        return new MessageResponse("Der er sendt en kode til telefonnummeret.");
    }

    /// <summary>Bekræfter og gemmer det nye telefonnummer med koden fra SMS'en.</summary>
    [HttpPost("phone/confirm")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<ActionResult<SettingsDto>> ConfirmPhoneChange(ConfirmPhoneRequest request) =>
        await profileService.ConfirmPhoneChangeAsync(request);
}
