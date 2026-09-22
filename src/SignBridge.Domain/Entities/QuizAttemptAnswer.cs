using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class QuizAttemptAnswer : Entity
{
    public Guid QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public Guid SelectedAnswerId { get; set; }
    public Answer SelectedAnswer { get; set; } = null!;

    public bool IsCorrect { get; set; }
}
