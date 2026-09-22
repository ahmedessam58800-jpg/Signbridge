namespace SignBridge.Application.DTOs.Content;

public sealed record CreateCourseRequest(string Title, string Description, string? CoverImageUrl);
public sealed record UpdateCourseRequest(string Title, string Description, string? CoverImageUrl);
public sealed record CreateLevelRequest(string Title, int Order);
public sealed record CreateLessonRequest(
    string Title,
    string Summary,
    string? VideoUrl,
    string? ThumbnailUrl,
    int DurationMinutes,
    int Order,
    int XpReward,
    int MinimumPassingScore);
public sealed record UpdateLessonRequest(
    string Title,
    string Summary,
    string? VideoUrl,
    string? ThumbnailUrl,
    int DurationMinutes,
    int Order,
    int XpReward,
    int MinimumPassingScore);
public sealed record CreateQuizRequest(string Title);
public sealed record CreateQuestionRequest(string Text, int Order, IReadOnlyList<CreateAnswerRequest> Answers);
public sealed record CreateAnswerRequest(string Text, bool IsCorrect);
public sealed record ResourceCreatedResponse(Guid Id);

public sealed record ContentCourseDto(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    bool IsPublished,
    int Levels,
    int Lessons,
    DateTime CreatedAtUtc);

public sealed record ContentCourseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    bool IsPublished,
    IReadOnlyList<ContentLevelDto> Levels);

public sealed record ContentLevelDto(
    Guid Id,
    string Title,
    int Order,
    IReadOnlyList<ContentLessonDto> Lessons);

public sealed record ContentLessonDto(
    Guid Id,
    string Title,
    string Summary,
    string? VideoUrl,
    string? ThumbnailUrl,
    int DurationMinutes,
    int Order,
    int XpReward,
    int MinimumPassingScore,
    bool IsPublished,
    ContentQuizDto? Quiz);

public sealed record ContentQuizDto(
    Guid Id,
    string Title,
    IReadOnlyList<ContentQuestionDto> Questions);

public sealed record ContentQuestionDto(
    Guid Id,
    string Text,
    int Order,
    IReadOnlyList<ContentAnswerDto> Answers);

public sealed record ContentAnswerDto(Guid Id, string Text, bool IsCorrect);


public sealed record CourseManagementDto(
    Guid Id,
    string Title,
    string Description,
    bool IsPublished,
    int Levels,
    int Lessons);

public sealed record LevelManagementDto(
    Guid Id,
    string Title,
    int Order,
    IReadOnlyList<LessonManagementDto> Lessons);

public sealed record LessonManagementDto(
    Guid Id,
    string Title,
    int Order,
    bool IsPublished,
    bool HasQuiz);
