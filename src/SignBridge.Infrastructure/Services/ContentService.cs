using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Content;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class ContentService(AppDbContext db) : IContentService
{
    public async Task<IReadOnlyList<ContentCourseDto>> GetCoursesAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.Courses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(term) || x.Description.ToLower().Contains(term));
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ContentCourseDto(
                x.Id, x.Title, x.Description, x.CoverImageUrl, x.IsPublished,
                x.Levels.Count,
                x.Levels.SelectMany(l => l.Lessons).Count(),
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<ContentCourseDetailsDto> GetCourseAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await db.Courses.AsNoTracking()
            .Include(x => x.Levels).ThenInclude(x => x.Lessons).ThenInclude(x => x.Quiz).ThenInclude(x => x!.Questions).ThenInclude(x => x.Answers)
            .SingleOrDefaultAsync(x => x.Id == courseId, cancellationToken)
            ?? throw new NotFoundException("Course not found.");

        return new ContentCourseDetailsDto(
            course.Id, course.Title, course.Description, course.CoverImageUrl, course.IsPublished,
            course.Levels.OrderBy(x => x.Order).Select(level => new ContentLevelDto(
                level.Id, level.Title, level.Order,
                level.Lessons.OrderBy(x => x.Order).Select(lesson => new ContentLessonDto(
                    lesson.Id, lesson.Title, lesson.Summary, lesson.VideoUrl, lesson.ThumbnailUrl,
                    lesson.DurationMinutes, lesson.Order, lesson.XpReward, lesson.MinimumPassingScore,
                    lesson.IsPublished,
                    lesson.Quiz is null ? null : new ContentQuizDto(
                        lesson.Quiz.Id, lesson.Quiz.Title,
                        lesson.Quiz.Questions.OrderBy(q => q.Order).Select(q => new ContentQuestionDto(
                            q.Id, q.Text, q.Order,
                            q.Answers.Select(a => new ContentAnswerDto(a.Id, a.Text, a.IsCorrect)).ToList()
                        )).ToList()
                    )
                )).ToList()
            )).ToList()
        );
    }

    public async Task<Guid> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Course title"); RequireText(request.Description, "Course description");
        var course = new Course { Title=request.Title.Trim(), Description=request.Description.Trim(), CoverImageUrl=request.CoverImageUrl?.Trim() };
        db.Courses.Add(course); await db.SaveChangesAsync(cancellationToken); return course.Id;
    }

    public async Task UpdateCourseAsync(Guid courseId, UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Course title"); RequireText(request.Description, "Course description");
        var course = await db.Courses.SingleOrDefaultAsync(x => x.Id == courseId, cancellationToken) ?? throw new NotFoundException("Course not found.");
        course.Title=request.Title.Trim(); course.Description=request.Description.Trim(); course.CoverImageUrl=request.CoverImageUrl?.Trim(); course.UpdatedAtUtc=DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await db.Courses.SingleOrDefaultAsync(x => x.Id == courseId, cancellationToken) ?? throw new NotFoundException("Course not found.");
        db.Courses.Remove(course); await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> CreateLevelAsync(Guid courseId, CreateLevelRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Level title");
        if (!await db.Courses.AnyAsync(x => x.Id == courseId, cancellationToken)) throw new NotFoundException("Course not found.");
        if (await db.Levels.AnyAsync(x => x.CourseId == courseId && x.Order == request.Order, cancellationToken)) throw new ConflictException("A level with this order already exists in the course.");
        var level = new Level { CourseId=courseId, Title=request.Title.Trim(), Order=request.Order };
        db.Levels.Add(level); await db.SaveChangesAsync(cancellationToken); return level.Id;
    }

    public async Task<Guid> CreateLessonAsync(Guid levelId, CreateLessonRequest request, CancellationToken cancellationToken)
    {
        ValidateLesson(request.Title, request.Summary, request.MinimumPassingScore, request.XpReward);
        if (!await db.Levels.AnyAsync(x => x.Id == levelId, cancellationToken)) throw new NotFoundException("Level not found.");
        if (await db.Lessons.AnyAsync(x => x.LevelId == levelId && x.Order == request.Order, cancellationToken)) throw new ConflictException("A lesson with this order already exists in the level.");
        var lesson = new Lesson { LevelId=levelId, Title=request.Title.Trim(), Summary=request.Summary.Trim(), VideoUrl=request.VideoUrl?.Trim(), ThumbnailUrl=request.ThumbnailUrl?.Trim(), DurationMinutes=Math.Max(0, request.DurationMinutes), Order=request.Order, XpReward=request.XpReward, MinimumPassingScore=request.MinimumPassingScore };
        db.Lessons.Add(lesson); await db.SaveChangesAsync(cancellationToken); return lesson.Id;
    }

    public async Task UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        ValidateLesson(request.Title, request.Summary, request.MinimumPassingScore, request.XpReward);
        var lesson=await db.Lessons.SingleOrDefaultAsync(x=>x.Id==lessonId,cancellationToken) ?? throw new NotFoundException("Lesson not found.");
        if (await db.Lessons.AnyAsync(x=>x.LevelId==lesson.LevelId && x.Id!=lessonId && x.Order==request.Order,cancellationToken)) throw new ConflictException("A lesson with this order already exists in the level.");
        lesson.Title=request.Title.Trim(); lesson.Summary=request.Summary.Trim(); lesson.VideoUrl=request.VideoUrl?.Trim(); lesson.ThumbnailUrl=request.ThumbnailUrl?.Trim(); lesson.DurationMinutes=Math.Max(0,request.DurationMinutes); lesson.Order=request.Order; lesson.XpReward=request.XpReward; lesson.MinimumPassingScore=request.MinimumPassingScore; lesson.UpdatedAtUtc=DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteLessonAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson=await db.Lessons.SingleOrDefaultAsync(x=>x.Id==lessonId,cancellationToken) ?? throw new NotFoundException("Lesson not found.");
        db.Lessons.Remove(lesson); await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> CreateQuizAsync(Guid lessonId, CreateQuizRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Quiz title");
        if (!await db.Lessons.AnyAsync(x => x.Id == lessonId, cancellationToken)) throw new NotFoundException("Lesson not found.");
        if (await db.Quizzes.AnyAsync(x => x.LessonId == lessonId, cancellationToken)) throw new ConflictException("This lesson already has a quiz.");
        var quiz = new Quiz { LessonId=lessonId, Title=request.Title.Trim() }; db.Quizzes.Add(quiz); await db.SaveChangesAsync(cancellationToken); return quiz.Id;
    }

    public async Task<Guid> CreateQuestionAsync(Guid quizId, CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Text, "Question");
        if (request.Answers.Count < 2) throw new ValidationException("A question needs at least two answers.");
        if (request.Answers.Count(x => x.IsCorrect) != 1) throw new ValidationException("A question must have exactly one correct answer.");
        if (!await db.Quizzes.AnyAsync(x => x.Id == quizId, cancellationToken)) throw new NotFoundException("Quiz not found.");
        if (await db.Questions.AnyAsync(x => x.QuizId == quizId && x.Order == request.Order, cancellationToken)) throw new ConflictException("A question with this order already exists in the quiz.");
        var question = new Question { QuizId=quizId, Text=request.Text.Trim(), Order=request.Order, Answers=request.Answers.Select(x=>new Answer{Text=x.Text.Trim(),IsCorrect=x.IsCorrect}).ToList() };
        if (question.Answers.Any(x => string.IsNullOrWhiteSpace(x.Text))) throw new ValidationException("Answer text cannot be empty.");
        db.Questions.Add(question); await db.SaveChangesAsync(cancellationToken); return question.Id;
    }

    public async Task SetCoursePublishedAsync(Guid courseId, bool isPublished, CancellationToken cancellationToken)
    {
        var course=await db.Courses.SingleOrDefaultAsync(x=>x.Id==courseId,cancellationToken) ?? throw new NotFoundException("Course not found."); course.IsPublished=isPublished; course.UpdatedAtUtc=DateTime.UtcNow; await db.SaveChangesAsync(cancellationToken);
    }
    public async Task SetLessonPublishedAsync(Guid lessonId, bool isPublished, CancellationToken cancellationToken)
    {
        var lesson=await db.Lessons.SingleOrDefaultAsync(x=>x.Id==lessonId,cancellationToken) ?? throw new NotFoundException("Lesson not found."); lesson.IsPublished=isPublished; lesson.UpdatedAtUtc=DateTime.UtcNow; await db.SaveChangesAsync(cancellationToken);
    }
    private static void ValidateLesson(string title,string summary,int score,int xp){ RequireText(title,"Lesson title"); RequireText(summary,"Lesson summary"); if(score is <1 or >100) throw new ValidationException("Minimum passing score must be between 1 and 100."); if(xp<0) throw new ValidationException("XP reward cannot be negative."); }
    private static void RequireText(string value,string fieldName){ if(string.IsNullOrWhiteSpace(value)) throw new ValidationException($"{fieldName} is required."); }
}
