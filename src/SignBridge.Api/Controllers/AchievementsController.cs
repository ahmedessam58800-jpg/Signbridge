using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Api.Extensions;
using SignBridge.Application.DTOs.Achievements;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/achievements")]
[Authorize(Roles = "Child")]
public sealed class AchievementsController(IAchievementService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AchievementDto>>> Get(CancellationToken cancellationToken)
    {
        return Ok(await service.GetForUserAsync(User.GetUserId(), cancellationToken));
    }
}
