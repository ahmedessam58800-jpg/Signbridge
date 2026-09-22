using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Application.DTOs.Admin;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        return Ok(await adminService.GetDashboardAsync(cancellationToken));
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetUsers(CancellationToken cancellationToken)
    {
        return Ok(await adminService.GetUsersAsync(cancellationToken));
    }
}
