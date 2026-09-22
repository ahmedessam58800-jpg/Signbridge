namespace SignBridge.Application.DTOs.Learning;

public sealed record CourseSummaryDto(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    int TotalLessons,
    int CompletedLessons,
    int ProgressPercent);

public sealed record CourseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    IReadOnlyList<LevelDto> Levels);

public sealed record LevelDto(
    Guid Id,
    string Title,
    int Order,
    IReadOnlyList<LessonListItemDto> Lessons);

public sealed record LessonListItemDto(
    Guid Id,
    string Title,
    string? ThumbnailUrl,
    int DurationMinutes,
    int Order,
    bool IsUnlocked,
    bool IsCompleted,
    int BestScore);

public sealed record LessonDetailsDto(
    Guid Id,
    string Title,
    string Summary,
    string? VideoUrl,
    string? ThumbnailUrl,
    int DurationMinutes,
    int XpReward,
    int MinimumPassingScore,
    bool IsFavorite,
    bool IsCompleted,
    int BestScore);

public sealed record FavoriteLessonDto(
    Guid Id,
    string Title,
    string? ThumbnailUrl,
    string CourseTitle,
    string LevelTitle);
