namespace SignBridge.Application.DTOs.Quiz;

public sealed record QuizDto(
    Guid Id,
    string Title,
    IReadOnlyList<QuestionDto> Questions);

public sealed record QuestionDto(
    Guid Id,
    string Text,
    int Order,
    IReadOnlyList<AnswerOptionDto> Answers);

public sealed record AnswerOptionDto(Guid Id, string Text);

public sealed record SubmitQuizRequest(IReadOnlyList<SubmitAnswerRequest> Answers);

public sealed record SubmitAnswerRequest(Guid QuestionId, Guid AnswerId);

public sealed record QuizResultDto(
    int Score,
    bool Passed,
    int CorrectAnswers,
    int TotalQuestions,
    bool LessonCompleted,
    int AwardedXp);
