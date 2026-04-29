using Application.Features.StudentCourse.Dtos;
using Application.Features.Tenants.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ILiveSessionRepository
    {
        Task<bool> LiveSessionIsExistAsync(int sessionId, int courseId, CancellationToken cancellationToken);
        Task<LiveSession?> GetLiveSessionAsync(int sessionId, CancellationToken cancellationToken);
        Task<LiveSession?> GetByZoomMeetingIdAsync(string zoomMeetingId, CancellationToken cancellationToken);
        Task<List<LiveSessionDto>> GetLiveSessionsByTenantIdAsync(int tenantId, CancellationToken cancellationToken);
        Task<LiveSessionDto> GetLiveSessionBySessionIdAsync(int sessionId, int tenantId, CancellationToken cancellationToken);
        Task CreateAsync(LiveSession session, CancellationToken cancellationToken);
        Task DeleteAsync(int SessionId, CancellationToken cancellationToken);
        Task<GetLiveSessionsStatisticsResponse> GetStatisticsAsync(string userId, int tenantId, CancellationToken cancellationToken);
        Task<List<StudentCourseLiveSessionsDto>> GetStudentCourseLiveSessionsAsync(int courseId, CancellationToken cancellationToken);
        Task<StudentCourseLiveSessionDto> GetStudentCourseLiveSessionAsync(int sessionId, int courseId, CancellationToken cancellationToken);
    }
}