using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Parent;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Domain.Enums;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class ParentService(AppDbContext db) : IParentService
{
    public async Task LinkChildAsync(Guid parentId, LinkChildRequest request, CancellationToken cancellationToken)
    {
        var email = request.ChildEmail.Trim().ToLowerInvariant();
        var code = request.LinkCode.Trim().ToUpperInvariant();

        var child = await db.Users.SingleOrDefaultAsync(
            x => x.Email == email && x.Role == UserRole.Child,
            cancellationToken)
            ?? throw new NotFoundException("Child account not found.");

        if (!string.Equals(child.LinkCode, code, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Invalid child link code.");

        var alreadyLinked = await db.ParentChildLinks
            .AnyAsync(x => x.ParentId == parentId && x.ChildId == child.Id, cancellationToken);

        if (alreadyLinked)
            return;

        db.ParentChildLinks.Add(new ParentChildLink
        {
            ParentId = parentId,
            ChildId = child.Id
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LinkedChildDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken)
    {
        return await db.ParentChildLinks
            .AsNoTracking()
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Child.FullName)
            .Select(x => new LinkedChildDto(
                x.ChildId,
                x.Child.FullName,
                x.Child.DateOfBirth,
                x.Child.TotalXp,
                x.Child.CurrentStreak))
            .ToListAsync(cancellationToken);
    }

    public async Task<ChildProgressDto> GetChildProgressAsync(
        Guid parentId,
        Guid childId,
        CancellationToken cancellationToken)
    {
        var linked = await db.ParentChildLinks
            .AnyAsync(x => x.ParentId == parentId && x.ChildId == childId, cancellationToken);

        if (!linked)
            throw new ForbiddenException("You are not linked to this child.");

        var child = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == childId && x.Role == UserRole.Child, cancellationToken)
            ?? throw new NotFoundException("Child not found.");

        var courses = await db.Courses
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .Include(x => x.Levels)
                .ThenInclude(x => x.Lessons.Where(l => l.IsPublished))
            .ToListAsync(cancellationToken);

        var progressRows = await db.LessonProgress
            .AsNoTracking()
            .Where(x => x.ChildId == childId)
            .ToListAsync(cancellationToken);

        var completedIds = progressRows
            .Where(x => x.Status == LessonProgressStatus.Completed)
            .Select(x => x.LessonId)
            .ToHashSet();

        var totalLessons = courses.SelectMany(x => x.Levels).SelectMany(x => x.Lessons).Count();
        var completedLessons = completedIds.Count;

        var courseProgress = courses.Select(course =>
        {
            var ids = course.Levels.SelectMany(x => x.Lessons).Select(x => x.Id).ToList();
            var completed = ids.Count(completedIds.Contains);

            return new CourseProgressDto(
                course.Id,
                course.Title,
                completed,
                ids.Count,
                Percentage(completed, ids.Count));
        }).ToList();

        var averageScore = await db.QuizAttempts
            .AsNoTracking()
            .Where(x => x.ChildId == childId)
            .Select(x => (double?)x.Score)
            .AverageAsync(cancellationToken) ?? 0;

        var recent = await db.LessonProgress
            .AsNoTracking()
            .Where(x =>
                x.ChildId == childId &&
                x.Status == LessonProgressStatus.Completed &&
                x.CompletedAtUtc != null)
            .OrderByDescending(x => x.CompletedAtUtc)
            .Take(5)
            .Select(x => new RecentCompletionDto(
                x.LessonId,
                x.Lesson.Title,
                x.CompletedAtUtc!.Value,
                x.BestScore))
            .ToListAsync(cancellationToken);

        return new ChildProgressDto(
            child.Id,
            child.FullName,
            completedLessons,
            totalLessons,
            Percentage(completedLessons, totalLessons),
            Math.Round(averageScore, 1),
            child.TotalXp,
            child.CurrentStreak,
            courseProgress,
            recent);
    }

    private static int Percentage(int completed, int total)
    {
        return total == 0 ? 0 : (int)Math.Round(completed * 100d / total);
    }
}
