using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Application.DTOs.Dictionary;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/dictionary")]
[Authorize]
public sealed class DictionaryController(IDictionaryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SignEntryDto>>> Search(
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] string? difficulty,
        CancellationToken cancellationToken)
    {
        return Ok(await service.SearchAsync(q, category, difficulty, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<object>> Create(
        CreateSignEntryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(request, cancellationToken);
        return Ok(new { id });
    }
}
