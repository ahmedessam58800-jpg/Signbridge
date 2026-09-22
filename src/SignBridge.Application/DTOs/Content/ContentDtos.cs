namespace SignBridge.Application.DTOs.Content;

public sealed record CreateCourseRequest(
    string Title,
    string Description,
    string? CoverImageUrl);

public sealed record CreateLevelRequest(
    string Title,
    int Order);

public sealed record CreateLessonRequest(
    string Title,
    string Summary,
    string? VideoUrl,
    string? ThumbnailUrl,
    int DurationMinutes,
    int Order,
    int XpReward,
    int MinimumPassingScore);

public sealed record CreateQuizRequest(string Title);

public sealed record CreateQuestionRequest(
    string Text,
    int Order,
    IReadOnlyList<CreateAnswerRequest> Answers);

public sealed record CreateAnswerRequest(
    string Text,
    bool IsCorrect);

public sealed record ResourceCreatedResponse(Guid Id);
