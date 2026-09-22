using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Favorite : Entity
{
    public Guid ChildId { get; set; }
    public User Child { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}
