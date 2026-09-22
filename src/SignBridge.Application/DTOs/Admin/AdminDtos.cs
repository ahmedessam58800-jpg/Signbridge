namespace SignBridge.Application.DTOs.Admin;

public sealed record AdminDashboardDto(
    int Users,
    int Children,
    int Parents,
    int Teachers,
    int Courses,
    int PublishedCourses,
    int CompletedLessons,
    int QuizAttempts);

public sealed record UserListItemDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    int TotalXp,
    DateTime CreatedAtUtc);
