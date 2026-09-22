using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Course : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; }

    public ICollection<Level> Levels { get; set; } = new List<Level>();
}
