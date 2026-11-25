using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPut("settings")]
    public async Task<ActionResult<SettingsDto>> UpdateSettings(UpdateSettingsRequest request) =>
        await profileService.UpdateSettingsAsync(request);
}
