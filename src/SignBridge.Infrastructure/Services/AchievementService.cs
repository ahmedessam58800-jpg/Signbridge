using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Achievements;
using SignBridge.Application.Interfaces;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class AchievementService(AppDbContext db) : IAchievementService
{
    public async Task<IReadOnlyList<AchievementDto>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var xp = await db.Users.Where(x => x.Id == userId)
            .Select(x => x.TotalXp)
            .SingleAsync(cancellationToken);

        return await db.Achievements.AsNoTracking()
            .OrderBy(x => x.RequiredXp)
            .Select(x => new AchievementDto(
                x.Id, x.Name, x.Description, x.Icon, x.RequiredXp, xp >= x.RequiredXp))
            .ToListAsync(cancellationToken);
    }
}
