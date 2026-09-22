using SignBridge.Application.DTOs.Content;

namespace SignBridge.Application.Interfaces;

public interface IContentService
{
    Task<IReadOnlyList<ContentCourseDto>> GetCoursesAsync(string? search, CancellationToken cancellationToken);
    Task<ContentCourseDetailsDto> GetCourseAsync(Guid courseId, CancellationToken cancellationToken);
    Task<Guid> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken);
    Task UpdateCourseAsync(Guid courseId, UpdateCourseRequest request, CancellationToken cancellationToken);
    Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
    Task<Guid> CreateLevelAsync(Guid courseId, CreateLevelRequest request, CancellationToken cancellationToken);
    Task<Guid> CreateLessonAsync(Guid levelId, CreateLessonRequest request, CancellationToken cancellationToken);
    Task UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request, CancellationToken cancellationToken);
    Task DeleteLessonAsync(Guid lessonId, CancellationToken cancellationToken);
    Task<Guid> CreateQuizAsync(Guid lessonId, CreateQuizRequest request, CancellationToken cancellationToken);
    Task<Guid> CreateQuestionAsync(Guid quizId, CreateQuestionRequest request, CancellationToken cancellationToken);
    Task SetCoursePublishedAsync(Guid courseId, bool isPublished, CancellationToken cancellationToken);
    Task SetLessonPublishedAsync(Guid lessonId, bool isPublished, CancellationToken cancellationToken);
}
