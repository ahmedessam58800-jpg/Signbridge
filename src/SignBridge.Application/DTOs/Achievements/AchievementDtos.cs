namespace SignBridge.Application.DTOs.Achievements;

public sealed record AchievementDto(
    Guid Id,
    string Name,
    string Description,
    string Icon,
    int RequiredXp,
    bool Earned);
