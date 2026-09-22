using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Admin;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Enums;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class AdminService(AppDbContext db) : IAdminService
{
    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        return new AdminDashboardDto(
            Users: await db.Users.CountAsync(cancellationToken),
            Children: await db.Users.CountAsync(x => x.Role == UserRole.Child, cancellationToken),
            Parents: await db.Users.CountAsync(x => x.Role == UserRole.Parent, cancellationToken),
            Teachers: await db.Users.CountAsync(x => x.Role == UserRole.Teacher, cancellationToken),
            Courses: await db.Courses.CountAsync(cancellationToken),
            PublishedCourses: await db.Courses.CountAsync(x => x.IsPublished, cancellationToken),
            CompletedLessons: await db.LessonProgress.CountAsync(
                x => x.Status == LessonProgressStatus.Completed, cancellationToken),
            QuizAttempts: await db.QuizAttempts.CountAsync(cancellationToken));
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return await db.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new UserListItemDto(
                x.Id,
                x.FullName,
                x.Email,
                x.Role.ToString(),
                x.IsActive,
                x.TotalXp,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
