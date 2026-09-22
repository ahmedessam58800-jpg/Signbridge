using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Question : Entity
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
