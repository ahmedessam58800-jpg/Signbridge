using SignBridge.Application.DTOs.Admin;

namespace SignBridge.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(CancellationToken cancellationToken);
}
