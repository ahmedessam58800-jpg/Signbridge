using SignBridge.Application.DTOs.Parent;

namespace SignBridge.Application.Interfaces;

public interface IParentService
{
    Task LinkChildAsync(Guid parentId, LinkChildRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<LinkedChildDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken);
    Task<ChildProgressDto> GetChildProgressAsync(Guid parentId, Guid childId, CancellationToken cancellationToken);
}
