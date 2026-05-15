using Application.Features.Plan.DTOs;

namespace Application.Contracts.Repositories
{
    public interface IPlanRepository
    {
        Task<IEnumerable<PlanDto>> GetAllPlansWithDetailsAsync(CancellationToken cancellationToken);
        Task<Guid> GetFreePlanPricingIdAsync(CancellationToken cancellationToken);
        Task<List<Guid>> GetPlanFeatureIdsAsync(Guid PlanId, CancellationToken cancellationToken);
        Task<Guid> GetPlanIdAsync(Guid PlanPricingId, CancellationToken cancellationToken);
        Task<Guid> GetFeatureIdAsync(string featureName, CancellationToken cancellationToken);
        Task<Guid> GetPlanFeatureIdByFeatureIdAsync(Guid PlanId, Guid FeatureId, CancellationToken cancellationToken);
        Task<int> GetFeatureLimitAsync(Guid planFeatureId, CancellationToken cancellationToken);
        Task<(int Used, int Limit)?> GetFeatureUsageInfoAsync(int tenantId, string featureName, CancellationToken cancellationToken);
    }
}
