using SignBridge.Domain.Common;
using SignBridge.Domain.Enums;

namespace SignBridge.Domain.Entities;

public sealed class User : Entity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? LinkCode { get; set; }
    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastLearningDateUtc { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ParentChildLink> ParentLinks { get; set; } = new List<ParentChildLink>();
    public ICollection<ParentChildLink> ChildLinks { get; set; } = new List<ParentChildLink>();
    public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
