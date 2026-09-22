using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Content;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class ContentService(AppDbContext db) : IContentService
{
    public async Task<Guid> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Course title");
        RequireText(request.Description, "Course description");

        var course = new Course
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim()
        };

        db.Courses.Add(course);
        await db.SaveChangesAsync(cancellationToken);
        return course.Id;
    }

    public async Task<Guid> CreateLevelAsync(Guid courseId, CreateLevelRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Level title");

        if (!await db.Courses.AnyAsync(x => x.Id == courseId, cancellationToken))
            throw new NotFoundException("Course not found.");

        if (await db.Levels.AnyAsync(x => x.CourseId == courseId && x.Order == request.Order, cancellationToken))
            throw new ConflictException("A level with this order already exists in the course.");

        var level = new Level
        {
            CourseId = courseId,
            Title = request.Title.Trim(),
            Order = request.Order
        };

        db.Levels.Add(level);
        await db.SaveChangesAsync(cancellationToken);
        return level.Id;
    }

    public async Task<Guid> CreateLessonAsync(Guid levelId, CreateLessonRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Lesson title");
        RequireText(request.Summary, "Lesson summary");

        if (request.MinimumPassingScore is < 1 or > 100)
            throw new ValidationException("Minimum passing score must be between 1 and 100.");

        if (request.XpReward < 0)
            throw new ValidationException("XP reward cannot be negative.");

        if (!await db.Levels.AnyAsync(x => x.Id == levelId, cancellationToken))
            throw new NotFoundException("Level not found.");

        if (await db.Lessons.AnyAsync(x => x.LevelId == levelId && x.Order == request.Order, cancellationToken))
            throw new ConflictException("A lesson with this order already exists in the level.");

        var lesson = new Lesson
        {
            LevelId = levelId,
            Title = request.Title.Trim(),
            Summary = request.Summary.Trim(),
            VideoUrl = request.VideoUrl?.Trim(),
            ThumbnailUrl = request.ThumbnailUrl?.Trim(),
            DurationMinutes = Math.Max(0, request.DurationMinutes),
            Order = request.Order,
            XpReward = request.XpReward,
            MinimumPassingScore = request.MinimumPassingScore
        };

        db.Lessons.Add(lesson);
        await db.SaveChangesAsync(cancellationToken);
        return lesson.Id;
    }

    public async Task<Guid> CreateQuizAsync(Guid lessonId, CreateQuizRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Title, "Quiz title");

        if (!await db.Lessons.AnyAsync(x => x.Id == lessonId, cancellationToken))
            throw new NotFoundException("Lesson not found.");

        if (await db.Quizzes.AnyAsync(x => x.LessonId == lessonId, cancellationToken))
            throw new ConflictException("This lesson already has a quiz.");

        var quiz = new Quiz
        {
            LessonId = lessonId,
            Title = request.Title.Trim()
        };

        db.Quizzes.Add(quiz);
        await db.SaveChangesAsync(cancellationToken);
        return quiz.Id;
    }

    public async Task<Guid> CreateQuestionAsync(Guid quizId, CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        RequireText(request.Text, "Question");

        if (request.Answers.Count < 2)
            throw new ValidationException("A question needs at least two answers.");

        if (request.Answers.Count(x => x.IsCorrect) != 1)
            throw new ValidationException("A question must have exactly one correct answer.");

        if (!await db.Quizzes.AnyAsync(x => x.Id == quizId, cancellationToken))
            throw new NotFoundException("Quiz not found.");

        if (await db.Questions.AnyAsync(x => x.QuizId == quizId && x.Order == request.Order, cancellationToken))
            throw new ConflictException("A question with this order already exists in the quiz.");

        var question = new Question
        {
            QuizId = quizId,
            Text = request.Text.Trim(),
            Order = request.Order,
            Answers = request.Answers.Select(x => new Answer
            {
                Text = x.Text.Trim(),
                IsCorrect = x.IsCorrect
            }).ToList()
        };

        if (question.Answers.Any(x => string.IsNullOrWhiteSpace(x.Text)))
            throw new ValidationException("Answer text cannot be empty.");

        db.Questions.Add(question);
        await db.SaveChangesAsync(cancellationToken);
        return question.Id;
    }

    public async Task SetCoursePublishedAsync(Guid courseId, bool isPublished, CancellationToken cancellationToken)
    {
        var course = await db.Courses.SingleOrDefaultAsync(x => x.Id == courseId, cancellationToken)
            ?? throw new NotFoundException("Course not found.");

        course.IsPublished = isPublished;
        course.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetLessonPublishedAsync(Guid lessonId, bool isPublished, CancellationToken cancellationToken)
    {
        var lesson = await db.Lessons.SingleOrDefaultAsync(x => x.Id == lessonId, cancellationToken)
            ?? throw new NotFoundException("Lesson not found.");

        lesson.IsPublished = isPublished;
        lesson.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    private static void RequireText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{fieldName} is required.");
    }
}
