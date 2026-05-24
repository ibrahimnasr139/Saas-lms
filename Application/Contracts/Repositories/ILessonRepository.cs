using Application.Features.Lessons.Dtos;
using Application.Features.StudentLessons.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ILessonRepository
    {
        Task<List<StudentViewsDto>> GetAllStudentsViewsAsync(int courseId, int itemId, CancellationToken cancellationToken);
        Task<List<ViewsOverTime>> GetViewsOverTimeAsync(int itemId, CancellationToken cancellationToken);
        Task<LessonOverviewDto?> GetLessonOverviewAsync(int itemId, CancellationToken cancellationToken);
        Task<DateTime> GetPeakActivityTimeAsync(int itemId, CancellationToken cancellationToken);
        Task<bool> IsFound(int id, int moduleId, int courseId, string subdomain, CancellationToken cancellationToken);
        Task<StudentLessonProgressDto> GetStudentLessonProgressAsync(int studentId, int courseId, int itemId, CancellationToken cancellationToken);
        Task<string> GetVideoIdAsync(int itemId, int courseId, CancellationToken cancellationToken);
        Task<StudentLessonTranscriptDto?> GetStudentLessonTranscriptAsync(string videoId, CancellationToken cancellationToken);
        Task<string> GetLessonNameAsync(int itemId, CancellationToken cancellationToken);
        Task CreateAiAssistantMessageAsync(List<AiAssistantMessage> messages, CancellationToken cancellationToken);
        Task<List<AiChatMessage>?> GetAiChatMessagesAsync(int itemId, int studentId, CancellationToken cancellationToken);
        Task<LessonContentDto?> GetLessonContentAsync(int courseId, int moduleId, int itemId, CancellationToken cancellationToken);
    }
}