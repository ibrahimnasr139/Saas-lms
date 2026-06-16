using Application.Features.Dashboards.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IDashboardRepository
    {
        public Task<List<PendingTaskDto>> GetPendingTasksAsync(string subdomain, CancellationToken cancellationToken);
    }
}