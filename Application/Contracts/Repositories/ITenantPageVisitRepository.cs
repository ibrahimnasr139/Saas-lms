namespace Application.Contracts.Repositories
{
    public interface ITenantPageVisitRepository
    {
        Task AddTenantPageVisitAsync(TenantPageVisit pageVisit, CancellationToken cancellationToken);
        Task<TenantPageVisit?> GetTenantPageVisitAsync(string subDomain, string pageUrl, Guid visitorId, CancellationToken cancellationToken);
    }
}