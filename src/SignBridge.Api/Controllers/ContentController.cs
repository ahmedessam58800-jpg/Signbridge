using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignBridge.Application.DTOs.Content;
using SignBridge.Application.Interfaces;

namespace SignBridge.Api.Controllers;

[ApiController]
[Route("api/content")]
[Authorize(Roles = "Admin,Teacher")]
public sealed class ContentController(IContentService contentService) : ControllerBase
{
    [HttpPost("courses")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateCourse(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var id = await contentService.CreateCourseAsync(request, cancellationToken);
        return Ok(new ResourceCreatedResponse(id));
    }

    [HttpPost("courses/{courseId:guid}/levels")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateLevel(
        Guid courseId,
        CreateLevelRequest request,
        CancellationToken cancellationToken)
    {
        var id = await contentService.CreateLevelAsync(courseId, request, cancellationToken);
        return Ok(new ResourceCreatedResponse(id));
    }

    [HttpPost("levels/{levelId:guid}/lessons")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateLesson(
        Guid levelId,
        CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var id = await contentService.CreateLessonAsync(levelId, request, cancellationToken);
        return Ok(new ResourceCreatedResponse(id));
    }

    [HttpPost("lessons/{lessonId:guid}/quiz")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateQuiz(
        Guid lessonId,
        CreateQuizRequest request,
        CancellationToken cancellationToken)
    {
        var id = await contentService.CreateQuizAsync(lessonId, request, cancellationToken);
        return Ok(new ResourceCreatedResponse(id));
    }

    [HttpPost("quizzes/{quizId:guid}/questions")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateQuestion(
        Guid quizId,
        CreateQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var id = await contentService.CreateQuestionAsync(quizId, request, cancellationToken);
        return Ok(new ResourceCreatedResponse(id));
    }

    [HttpPatch("courses/{courseId:guid}/publish")]
    public async Task<IActionResult> PublishCourse(
        Guid courseId,
        [FromQuery] bool published,
        CancellationToken cancellationToken)
    {
        await contentService.SetCoursePublishedAsync(courseId, published, cancellationToken);
        return NoContent();
    }

    [HttpPatch("lessons/{lessonId:guid}/publish")]
    public async Task<IActionResult> PublishLesson(
        Guid lessonId,
        [FromQuery] bool published,
        CancellationToken cancellationToken)
    {
        await contentService.SetLessonPublishedAsync(lessonId, published, cancellationToken);
        return NoContent();
    }
}
