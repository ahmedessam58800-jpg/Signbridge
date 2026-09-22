using SignBridge.Domain.Common;
using SignBridge.Domain.Enums;

namespace SignBridge.Domain.Entities;

public sealed class LessonProgress : Entity
{
    public Guid ChildId { get; set; }
    public User Child { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public LessonProgressStatus Status { get; set; } = LessonProgressStatus.NotStarted;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int BestScore { get; set; }
}
