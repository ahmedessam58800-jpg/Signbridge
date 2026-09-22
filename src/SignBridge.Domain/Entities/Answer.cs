using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Answer : Entity
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
