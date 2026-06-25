using Application.Features.Tenants.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<int> CreateFreeSubcscription(int TenantId, Guid PlanPricingId, CancellationToken cancellationToken);
        Task<bool> HasActiveSubscriptionByTenantDomain(string subdomain, CancellationToken cancellationToken);
        Task<Guid> GetPlanPricingIdAsync(int tenantId, CancellationToken cancellationToken);
        Task<bool> TenantHasFeatureAsync(int tenantId, string featureKey, CancellationToken cancellationToken);
        Task<List<ExpiredSubscriptionDto>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken);
        Task<List<ExpiringSubscriptionDto>> GetSubscriptionsExpiringSoonAsync(CancellationToken cancellationToken);
        Task BulkExpireSubscriptionsAsync(List<int> subscriptionIds, CancellationToken cancellationToken);
    }
}
