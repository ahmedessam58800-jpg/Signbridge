using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class QuizAttempt : Entity
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid ChildId { get; set; }
    public User Child { get; set; } = null!;

    public int Score { get; set; }
    public bool Passed { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<QuizAttemptAnswer> Answers { get; set; } = new List<QuizAttemptAnswer>();
}
