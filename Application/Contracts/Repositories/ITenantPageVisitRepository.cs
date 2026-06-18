using Application.Features.Website.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantPageVisitRepository
    {
        Task AddTenantPageVisitAsync(TenantPageVisit pageVisit, CancellationToken cancellationToken);
        Task<TenantPageVisit?> GetTenantPageVisitAsync(string subDomain, string pageUrl, Guid visitorId, CancellationToken cancellationToken);
        Task<TenantPageVisit?> GetByVisitorAndPageAsync(Guid visitorId, int tenantId, string pageUrl, CancellationToken cancellationToken);
        Task<WebSiteStatisticsDto> GetTenantStatisticsAsync(string subDomain, CancellationToken cancellationToken);
    }
}