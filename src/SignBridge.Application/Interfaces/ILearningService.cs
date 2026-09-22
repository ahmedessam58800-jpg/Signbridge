using SignBridge.Application.DTOs.Learning;
using SignBridge.Application.DTOs.Quiz;

namespace SignBridge.Application.Interfaces;

public interface ILearningService
{
    Task<IReadOnlyList<CourseSummaryDto>> GetCoursesAsync(Guid childId, CancellationToken cancellationToken);
    Task<CourseDetailsDto> GetCourseAsync(Guid childId, Guid courseId, CancellationToken cancellationToken);
    Task<LessonDetailsDto> GetLessonAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken);
    Task<QuizDto> GetQuizAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken);
    Task<QuizResultDto> SubmitQuizAsync(Guid childId, Guid lessonId, SubmitQuizRequest request, CancellationToken cancellationToken);
    Task CompleteLessonAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken);
    Task AddFavoriteAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken);
    Task RemoveFavoriteAsync(Guid childId, Guid lessonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<FavoriteLessonDto>> GetFavoritesAsync(Guid childId, CancellationToken cancellationToken);
}
