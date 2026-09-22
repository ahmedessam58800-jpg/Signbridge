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
    [HttpGet("courses")]
    public async Task<ActionResult<IReadOnlyList<ContentCourseDto>>> GetCourses([FromQuery] string? search, CancellationToken ct) => Ok(await contentService.GetCoursesAsync(search, ct));
    [HttpGet("courses/{courseId:guid}")]
    public async Task<ActionResult<ContentCourseDetailsDto>> GetCourse(Guid courseId, CancellationToken ct) => Ok(await contentService.GetCourseAsync(courseId, ct));
    [HttpPost("courses")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateCourse(CreateCourseRequest request, CancellationToken ct) => Ok(new ResourceCreatedResponse(await contentService.CreateCourseAsync(request, ct)));
    [HttpPut("courses/{courseId:guid}")]
    public async Task<IActionResult> UpdateCourse(Guid courseId, UpdateCourseRequest request, CancellationToken ct){ await contentService.UpdateCourseAsync(courseId,request,ct); return NoContent(); }
    [HttpDelete("courses/{courseId:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid courseId,CancellationToken ct){ await contentService.DeleteCourseAsync(courseId,ct); return NoContent(); }
    [HttpPost("courses/{courseId:guid}/levels")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateLevel(Guid courseId, CreateLevelRequest request, CancellationToken ct) => Ok(new ResourceCreatedResponse(await contentService.CreateLevelAsync(courseId, request, ct)));
    [HttpPost("levels/{levelId:guid}/lessons")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateLesson(Guid levelId, CreateLessonRequest request, CancellationToken ct) => Ok(new ResourceCreatedResponse(await contentService.CreateLessonAsync(levelId, request, ct)));
    [HttpPut("lessons/{lessonId:guid}")]
    public async Task<IActionResult> UpdateLesson(Guid lessonId, UpdateLessonRequest request, CancellationToken ct){ await contentService.UpdateLessonAsync(lessonId,request,ct); return NoContent(); }
    [HttpDelete("lessons/{lessonId:guid}")]
    public async Task<IActionResult> DeleteLesson(Guid lessonId,CancellationToken ct){ await contentService.DeleteLessonAsync(lessonId,ct); return NoContent(); }
    [HttpPost("lessons/{lessonId:guid}/quiz")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateQuiz(Guid lessonId, CreateQuizRequest request, CancellationToken ct) => Ok(new ResourceCreatedResponse(await contentService.CreateQuizAsync(lessonId, request, ct)));
    [HttpPost("quizzes/{quizId:guid}/questions")]
    public async Task<ActionResult<ResourceCreatedResponse>> CreateQuestion(Guid quizId, CreateQuestionRequest request, CancellationToken ct) => Ok(new ResourceCreatedResponse(await contentService.CreateQuestionAsync(quizId, request, ct)));
    [HttpPatch("courses/{courseId:guid}/publish")]
    public async Task<IActionResult> PublishCourse(Guid courseId,[FromQuery] bool published,CancellationToken ct){ await contentService.SetCoursePublishedAsync(courseId,published,ct); return NoContent(); }
    [HttpPatch("lessons/{lessonId:guid}/publish")]
    public async Task<IActionResult> PublishLesson(Guid lessonId,[FromQuery] bool published,CancellationToken ct){ await contentService.SetLessonPublishedAsync(lessonId,published,ct); return NoContent(); }
}
