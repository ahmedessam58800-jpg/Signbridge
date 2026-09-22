using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Api.Extensions;
using SignBridge.Application.DTOs.Learning;
using SignBridge.Application.DTOs.Quiz;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/learning")]
[Authorize(Roles = "Child")]
public sealed class LearningController(ILearningService learningService) : ControllerBase
{
    [HttpGet("courses")]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> GetCourses(CancellationToken cancellationToken)
    {
        return Ok(await learningService.GetCoursesAsync(User.GetUserId(), cancellationToken));
    }

    [HttpGet("courses/{courseId:guid}")]
    public async Task<ActionResult<CourseDetailsDto>> GetCourse(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        return Ok(await learningService.GetCourseAsync(User.GetUserId(), courseId, cancellationToken));
    }

    [HttpGet("lessons/{lessonId:guid}")]
    public async Task<ActionResult<LessonDetailsDto>> GetLesson(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        return Ok(await learningService.GetLessonAsync(User.GetUserId(), lessonId, cancellationToken));
    }

    [HttpGet("lessons/{lessonId:guid}/quiz")]
    public async Task<ActionResult<QuizDto>> GetQuiz(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        return Ok(await learningService.GetQuizAsync(User.GetUserId(), lessonId, cancellationToken));
    }

    [HttpPost("lessons/{lessonId:guid}/quiz/submit")]
    public async Task<ActionResult<QuizResultDto>> SubmitQuiz(
        Guid lessonId,
        SubmitQuizRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await learningService.SubmitQuizAsync(
            User.GetUserId(),
            lessonId,
            request,
            cancellationToken));
    }

    [HttpPost("lessons/{lessonId:guid}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId, CancellationToken cancellationToken)
    {
        await learningService.CompleteLessonAsync(User.GetUserId(), lessonId, cancellationToken);
        return NoContent();
    }

    [HttpPost("lessons/{lessonId:guid}/favorite")]
    public async Task<IActionResult> AddFavorite(Guid lessonId, CancellationToken cancellationToken)
    {
        await learningService.AddFavoriteAsync(User.GetUserId(), lessonId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("lessons/{lessonId:guid}/favorite")]
    public async Task<IActionResult> RemoveFavorite(Guid lessonId, CancellationToken cancellationToken)
    {
        await learningService.RemoveFavoriteAsync(User.GetUserId(), lessonId, cancellationToken);
        return NoContent();
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<IReadOnlyList<FavoriteLessonDto>>> GetFavorites(CancellationToken cancellationToken)
    {
        return Ok(await learningService.GetFavoritesAsync(User.GetUserId(), cancellationToken));
    }
}
