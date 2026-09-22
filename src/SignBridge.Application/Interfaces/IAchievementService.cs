using SignBridge.Application.DTOs.Achievements;

namespace SignBridge.Application.Interfaces;

public interface IAchievementService
{
    Task<IReadOnlyList<AchievementDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
}
