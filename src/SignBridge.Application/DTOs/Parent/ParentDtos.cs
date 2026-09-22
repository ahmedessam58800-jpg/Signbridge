namespace SignBridge.Application.DTOs.Parent;

public sealed record LinkChildRequest(string ChildEmail, string LinkCode);

public sealed record LinkedChildDto(
    Guid Id,
    string FullName,
    DateOnly? DateOfBirth,
    int TotalXp,
    int CurrentStreak);

public sealed record ChildProgressDto(
    Guid ChildId,
    string ChildName,
    int CompletedLessons,
    int TotalLessons,
    int ProgressPercent,
    double AverageQuizScore,
    int TotalXp,
    int CurrentStreak,
    IReadOnlyList<CourseProgressDto> Courses,
    IReadOnlyList<RecentCompletionDto> RecentCompletions);

public sealed record CourseProgressDto(
    Guid CourseId,
    string CourseTitle,
    int CompletedLessons,
    int TotalLessons,
    int ProgressPercent);

public sealed record RecentCompletionDto(
    Guid LessonId,
    string LessonTitle,
    DateTime CompletedAtUtc,
    int BestScore);
