using Microsoft.EntityFrameworkCore;
using SignBridge.Domain.Entities;

namespace SignBridge.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ParentChildLink> ParentChildLinks => Set<ParentChildLink>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Level> Levels => Set<Level>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizAttemptAnswer> QuizAttemptAnswers => Set<QuizAttemptAnswer>();
    public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SignEntry> SignEntries => Set<SignEntry>();
    public DbSet<Achievement> Achievements => Set<Achievement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.FullName).HasMaxLength(150);
            entity.Property(x => x.LinkCode).HasMaxLength(20);
        });

        modelBuilder.Entity<ParentChildLink>(entity =>
        {
            entity.HasIndex(x => new { x.ParentId, x.ChildId }).IsUnique();

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.ParentLinks)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Child)
                .WithMany(x => x.ChildLinks)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Level>()
            .HasIndex(x => new { x.CourseId, x.Order })
            .IsUnique();

        modelBuilder.Entity<Lesson>()
            .HasIndex(x => new { x.LevelId, x.Order })
            .IsUnique();

        modelBuilder.Entity<Quiz>()
            .HasIndex(x => x.LessonId)
            .IsUnique();

        modelBuilder.Entity<Question>()
            .HasIndex(x => new { x.QuizId, x.Order })
            .IsUnique();

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasIndex(x => new { x.ChildId, x.LessonId }).IsUnique();
            entity.HasOne(x => x.Child)
                .WithMany(x => x.LessonProgress)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<QuizAttempt>()
            .HasOne(x => x.Child)
            .WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasIndex(x => new { x.ChildId, x.LessonId }).IsUnique();
            entity.HasOne(x => x.Child)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
