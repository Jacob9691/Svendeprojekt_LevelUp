using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.Api.Controllers;

[ApiController]
[Route("api/achievements")]
public class AchievementsController(IAchievementService achievementService) : ControllerBase
{
    /// <summary>Alle achievements med brugerens fremskridt og hvornår de er låst op.</summary>
    [HttpGet]
    public async Task<ActionResult<List<AchievementDto>>> GetAchievements() =>
        await achievementService.GetAchievementsAsync();
}

[ApiController]
[Route("api/challenges")]
public class ChallengesController(IChallengeService challengeService) : ControllerBase
{
    /// <summary>Alle udfordringer og brugerens status for hver.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ChallengeDto>>> GetChallenges() =>
        await challengeService.GetChallengesAsync();

    /// <summary>Starter udfordringen og lægger dens opgaver i kalenderen fra det valgte tidspunkt.</summary>
    [HttpPost("{id:int}/start")]
    public async Task<ActionResult<ChallengeDto>> Start(int id, StartChallengeRequest request) =>
        await challengeService.StartAsync(id, request);

    /// <summary>Opgiver en aktiv udfordring. Ikke-gennemførte opgaver fjernes fra kalenderen.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Abandon(int id)
    {
        await challengeService.AbandonAsync(id);
        return NoContent();
    }
}
