using SignBridge.Domain.Entities;
using SignBridge.Domain.Enums;

namespace SignBridge.UnitTests.Domain;

public sealed class LessonProgressTests
{
    [Fact]
    public void New_progress_starts_as_not_started()
    {
        var progress = new LessonProgress();

        Assert.Equal(LessonProgressStatus.NotStarted, progress.Status);
        Assert.Equal(0, progress.BestScore);
        Assert.Null(progress.CompletedAtUtc);
    }

    [Fact]
    public void Lesson_defaults_are_safe_for_new_content()
    {
        var lesson = new Lesson();

        Assert.False(lesson.IsPublished);
        Assert.Equal(80, lesson.MinimumPassingScore);
        Assert.Equal(50, lesson.XpReward);
    }
}
