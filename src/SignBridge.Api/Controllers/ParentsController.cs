using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Api.Extensions;
using SignBridge.Application.DTOs.Parent;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/parents")]
[Authorize(Roles = "Parent")]
public sealed class ParentsController(IParentService parentService) : ControllerBase
{
    [HttpPost("children/link")]
    public async Task<IActionResult> LinkChild(
        LinkChildRequest request,
        CancellationToken cancellationToken)
    {
        await parentService.LinkChildAsync(User.GetUserId(), request, cancellationToken);
        return NoContent();
    }

    [HttpGet("children")]
    public async Task<ActionResult<IReadOnlyList<LinkedChildDto>>> GetChildren(CancellationToken cancellationToken)
    {
        return Ok(await parentService.GetChildrenAsync(User.GetUserId(), cancellationToken));
    }

    [HttpGet("children/{childId:guid}/progress")]
    public async Task<ActionResult<ChildProgressDto>> GetChildProgress(
        Guid childId,
        CancellationToken cancellationToken)
    {
        return Ok(await parentService.GetChildProgressAsync(
            User.GetUserId(),
            childId,
            cancellationToken));
    }
}
