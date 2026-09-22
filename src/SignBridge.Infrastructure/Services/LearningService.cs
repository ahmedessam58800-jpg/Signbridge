using Microsoft.EntityFrameworkCore;
using SignBridge.Application.DTOs.Learning;
using SignBridge.Application.DTOs.Quiz;
using SignBridge.Application.Exceptions;
using SignBridge.Application.Interfaces;
using SignBridge.Domain.Entities;
using SignBridge.Domain.Enums;
using SignBridge.Infrastructure.Persistence;

namespace SignBridge.Infrastructure.Services;

internal sealed class LearningService(AppDbContext db) : ILearningService
{
    public async Task<IReadOnlyList<CourseSummaryDto>> GetCoursesAsync(Guid childId, CancellationToken cancellationToken)
    {
        var courses = await db.Courses
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .Include(x => x.Levels)
                .ThenInclude(x => x.Lessons.Where(l => l.IsPublished))
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        var completedIds = await db.LessonProgress
            .AsNoTracking()
            .Where(x => x.ChildId == childId && x.Status == LessonProgressStatus.Completed)
            .Select(x => x.LessonId)
            .ToHashSetAsync(cancellationToken);

        return courses.Select(course =>
        {
            var lessonIds = course.Levels.SelectMany(x => x.Lessons).Select(x => x.Id).ToList();
            var completed = lessonIds.Count(completedIds.Contains);
            var total = lessonIds.Count;

            return new CourseSummaryDto(
                course.Id,
                course.Title,
                course.Description,
                course.CoverImageUrl,
                total,
                completed,
                Percentage(completed, total));
        }).ToList();
    }

    public async Task<CourseDetailsDto> GetCourseAsync(Guid childId, Guid courseId, CancellationToken cancellationToken)
    {
        var course = await db.Courses
            .AsNoTracking()
            .Where(x => x.Id == courseId && x.IsPublished)
            .Include(x => x.Levels.OrderBy(l => l.Order))
                .ThenInclude(x => x.Lessons.Where(l => l.IsPublished).OrderBy(l => l.Order))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Course not found.");

        var lessonIds = course.Levels.SelectMany(x => x.Lessons).Select(x => x.Id).ToList();

        var progress = await db.LessonProgress
            .AsNoTracking()
            .Where(x => x.ChildId == childId && lessonIds.Contains(x.LessonId))
            .ToDictionaryAsync(x => x.LessonId, cancellationToken);

        var levels = new List<LevelDto>();

        foreach (var level in course.Levels.OrderBy(x => x.Order))
        {
            var orderedLessons = level.Lessons.OrderBy(x => x.Order).ToList();
            var lessonDtos = new List<LessonListItemDto>();

            for (var index = 0; index < orderedLessons.Count; index++)
            {
                var lesson = orderedLessons[index];
                progress.TryGetValue(lesson.Id, out var lessonProgress);

                var unlocked = index == 0 ||
                    (progress.TryGetValue(orderedLessons[index - 1].Id, out var previous) &&
                     previous.Status == LessonProgressStatus.Completed);

                lessonDtos.Add(new LessonListItemDto(
                    lesson.Id,
                    lesson.Title,
                    lesson.ThumbnailUrl,
                    lesson.DurationMinutes,
                    lesson.Order,
                    unlocked,
                    lessonProgress?.Status == LessonProgressStatus.Completed,
                    lessonProgress?.BestScore ?? 0));
            }

            levels.Add(new LevelDto(level.Id, level.Title, level.Order, lessonDtos));
        }

        return new CourseDetailsDto(
            course.Id,
            course.Title,
            course.Description,
            course.CoverImageUrl,
            levels);
    }

    public async Task<LessonDetailsDto> GetLessonAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson = await GetPublishedLessonAsync(lessonId, cancellationToken);
        await EnsureUnlockedAsync(childId, lesson, cancellationToken);

        var progress = await db.LessonProgress
            .SingleOrDefaultAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken);

        if (progress is null)
        {
            progress = new LessonProgress
            {
                ChildId = childId,
                LessonId = lessonId,
                Status = LessonProgressStatus.InProgress,
                StartedAtUtc = DateTime.UtcNow
            };

            db.LessonProgress.Add(progress);
            await db.SaveChangesAsync(cancellationToken);
        }
        else if (progress.Status == LessonProgressStatus.NotStarted)
        {
            progress.Status = LessonProgressStatus.InProgress;
            progress.StartedAtUtc ??= DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        var isFavorite = await db.Favorites
            .AnyAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken);

        return new LessonDetailsDto(
            lesson.Id,
            lesson.Title,
            lesson.Summary,
            lesson.VideoUrl,
            lesson.ThumbnailUrl,
            lesson.DurationMinutes,
            lesson.XpReward,
            lesson.MinimumPassingScore,
            isFavorite,
            progress.Status == LessonProgressStatus.Completed,
            progress.BestScore);
    }

    public async Task<QuizDto> GetQuizAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson = await GetPublishedLessonAsync(lessonId, cancellationToken);
        await EnsureUnlockedAsync(childId, lesson, cancellationToken);

        var quiz = await db.Quizzes
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .Include(x => x.Questions.OrderBy(q => q.Order))
                .ThenInclude(x => x.Answers)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("This lesson does not have a quiz.");

        return new QuizDto(
            quiz.Id,
            quiz.Title,
            quiz.Questions
                .OrderBy(x => x.Order)
                .Select(question => new QuestionDto(
                    question.Id,
                    question.Text,
                    question.Order,
                    question.Answers
                        .Select(answer => new AnswerOptionDto(answer.Id, answer.Text))
                        .ToList()))
                .ToList());
    }

    public async Task<QuizResultDto> SubmitQuizAsync(
        Guid childId,
        Guid lessonId,
        SubmitQuizRequest request,
        CancellationToken cancellationToken)
    {
        var lesson = await GetPublishedLessonAsync(lessonId, cancellationToken);
        await EnsureUnlockedAsync(childId, lesson, cancellationToken);

        var quiz = await db.Quizzes
            .Where(x => x.LessonId == lessonId)
            .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("This lesson does not have a quiz.");

        if (quiz.Questions.Count == 0)
            throw new ValidationException("Quiz has no questions.");

        if (request.Answers.Count != quiz.Questions.Count)
            throw new ValidationException("Every quiz question must be answered exactly once.");

        if (request.Answers.Select(x => x.QuestionId).Distinct().Count() != request.Answers.Count)
            throw new ValidationException("A question cannot be answered more than once.");

        var attempt = new QuizAttempt
        {
            QuizId = quiz.Id,
            ChildId = childId
        };

        var correct = 0;

        foreach (var question in quiz.Questions)
        {
            var submitted = request.Answers.SingleOrDefault(x => x.QuestionId == question.Id)
                ?? throw new ValidationException("Every quiz question must be answered.");

            var selectedAnswer = question.Answers.SingleOrDefault(x => x.Id == submitted.AnswerId)
                ?? throw new ValidationException("Selected answer does not belong to the question.");

            if (selectedAnswer.IsCorrect)
                correct++;

            attempt.Answers.Add(new QuizAttemptAnswer
            {
                QuestionId = question.Id,
                SelectedAnswerId = selectedAnswer.Id,
                IsCorrect = selectedAnswer.IsCorrect
            });
        }

        var score = (int)Math.Round(correct * 100d / quiz.Questions.Count);
        var passed = score >= lesson.MinimumPassingScore;

        attempt.Score = score;
        attempt.Passed = passed;

        db.QuizAttempts.Add(attempt);

        var progress = await db.LessonProgress
            .SingleOrDefaultAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken);

        if (progress is null)
        {
            progress = new LessonProgress
            {
                ChildId = childId,
                LessonId = lessonId,
                Status = LessonProgressStatus.InProgress,
                StartedAtUtc = DateTime.UtcNow
            };
            db.LessonProgress.Add(progress);
        }

        progress.BestScore = Math.Max(progress.BestScore, score);

        var awardedXp = 0;

        if (passed && progress.Status != LessonProgressStatus.Completed)
        {
            progress.Status = LessonProgressStatus.Completed;
            progress.CompletedAtUtc = DateTime.UtcNow;
            awardedXp = lesson.XpReward;

            var child = await db.Users.SingleAsync(x => x.Id == childId, cancellationToken);
            child.TotalXp += awardedXp;
            UpdateStreak(child);
        }

        await db.SaveChangesAsync(cancellationToken);

        return new QuizResultDto(
            score,
            passed,
            correct,
            quiz.Questions.Count,
            progress.Status == LessonProgressStatus.Completed,
            awardedXp);
    }

    public async Task CompleteLessonAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson = await GetPublishedLessonAsync(lessonId, cancellationToken);
        await EnsureUnlockedAsync(childId, lesson, cancellationToken);

        var hasQuiz = await db.Quizzes.AnyAsync(x => x.LessonId == lessonId, cancellationToken);
        if (hasQuiz)
            throw new ConflictException("This lesson has a quiz and must be completed by passing it.");

        var progress = await db.LessonProgress
            .SingleOrDefaultAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken);

        if (progress?.Status == LessonProgressStatus.Completed)
            return;

        if (progress is null)
        {
            progress = new LessonProgress
            {
                ChildId = childId,
                LessonId = lessonId,
                StartedAtUtc = DateTime.UtcNow
            };
            db.LessonProgress.Add(progress);
        }

        progress.Status = LessonProgressStatus.Completed;
        progress.CompletedAtUtc = DateTime.UtcNow;

        var child = await db.Users.SingleAsync(x => x.Id == childId, cancellationToken);
        child.TotalXp += lesson.XpReward;
        UpdateStreak(child);

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddFavoriteAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken)
    {
        var exists = await db.Lessons.AnyAsync(x => x.Id == lessonId && x.IsPublished, cancellationToken);
        if (!exists)
            throw new NotFoundException("Lesson not found.");

        if (await db.Favorites.AnyAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken))
            return;

        db.Favorites.Add(new Favorite { ChildId = childId, LessonId = lessonId });
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFavoriteAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken)
    {
        var favorite = await db.Favorites
            .SingleOrDefaultAsync(x => x.ChildId == childId && x.LessonId == lessonId, cancellationToken);

        if (favorite is null)
            return;

        db.Favorites.Remove(favorite);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FavoriteLessonDto>> GetFavoritesAsync(Guid childId, CancellationToken cancellationToken)
    {
        return await db.Favorites
            .AsNoTracking()
            .Where(x => x.ChildId == childId && x.Lesson.IsPublished && x.Lesson.Level.Course.IsPublished)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new FavoriteLessonDto(
                x.LessonId,
                x.Lesson.Title,
                x.Lesson.ThumbnailUrl,
                x.Lesson.Level.Course.Title,
                x.Lesson.Level.Title))
            .ToListAsync(cancellationToken);
    }

    private async Task<Lesson> GetPublishedLessonAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return await db.Lessons
            .Include(x => x.Level)
                .ThenInclude(x => x.Course)
            .SingleOrDefaultAsync(
                x => x.Id == lessonId && x.IsPublished && x.Level.Course.IsPublished,
                cancellationToken)
            ?? throw new NotFoundException("Lesson not found.");
    }

    private async Task EnsureUnlockedAsync(Guid childId, Lesson lesson, CancellationToken cancellationToken)
    {
        var previousLesson = await db.Lessons
            .AsNoTracking()
            .Where(x =>
                x.LevelId == lesson.LevelId &&
                x.IsPublished &&
                x.Order < lesson.Order)
            .OrderByDescending(x => x.Order)
            .FirstOrDefaultAsync(cancellationToken);

        if (previousLesson is null)
            return;

        var previousCompleted = await db.LessonProgress.AnyAsync(
            x =>
                x.ChildId == childId &&
                x.LessonId == previousLesson.Id &&
                x.Status == LessonProgressStatus.Completed,
            cancellationToken);

        if (!previousCompleted)
            throw new ForbiddenException("Complete the previous lesson before opening this one.");
    }

    private static void UpdateStreak(User child)
    {
        var today = DateTime.UtcNow.Date;
        var lastDate = child.LastLearningDateUtc?.Date;

        if (lastDate == today)
            return;

        child.CurrentStreak = lastDate == today.AddDays(-1)
            ? child.CurrentStreak + 1
            : 1;

        child.LastLearningDateUtc = DateTime.UtcNow;
    }

    private static int Percentage(int completed, int total)
    {
        return total == 0 ? 0 : (int)Math.Round(completed * 100d / total);
    }
}
