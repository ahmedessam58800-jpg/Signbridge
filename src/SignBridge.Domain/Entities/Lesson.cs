using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class Lesson : Entity
{
    public Guid LevelId { get; set; }
    public Level Level { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DurationMinutes { get; set; }
    public int Order { get; set; }
    public int XpReward { get; set; } = 50;
    public int MinimumPassingScore { get; set; } = 80;
    public bool IsPublished { get; set; }

    public Quiz? Quiz { get; set; }
    public ICollection<LessonProgress> Progress { get; set; } = new List<LessonProgress>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
