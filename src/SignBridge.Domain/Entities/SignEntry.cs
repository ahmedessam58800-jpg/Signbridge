using SignBridge.Domain.Common;

namespace SignBridge.Domain.Entities;

public sealed class SignEntry : Entity
{
    public string Word { get; set; } = string.Empty;
    public string ArabicWord { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Beginner";
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; } = true;
}
