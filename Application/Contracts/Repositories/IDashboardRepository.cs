using Application.Features.Dashboards.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IDashboardRepository
    {
        public Task<List<PendingTaskDto>> GetPendingTasksAsync(string subdomain, CancellationToken cancellationToken);
        public Task<QuickAnalyticsDto> GetQuickAnalyticsAsync(string subdomain, CancellationToken cancellationToken);
        public Task<List<UpcomingSessionsDto>> GetUpcomingSessionsAsync(string subdomain, CancellationToken cancellationToken);
        public Task<List<TopStudentsPerformanceDto>> GetTopStudentsPerformanceAsync(string subdomain, CancellationToken cancellationToken);
        public Task<DashboardPerformanceDto> GetPerformanceAsync(string subdomain, CancellationToken cancellationToken);
    }
}