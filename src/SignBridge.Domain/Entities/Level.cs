using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Level : Entity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
