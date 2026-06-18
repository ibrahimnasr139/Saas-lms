namespace Application.Contracts.Repositories
{
    public interface ITenantPageVisitRepository
    {
        Task AddTenantPageVisitAsync(TenantPageVisit pageVisit , CancellationToken cancellationToken);
    }
}