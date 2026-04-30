using Application.Features.Plan.DTOs;

namespace Application.Contracts.Repositories
{
    public interface IPlanRepository
    {
        Task<IEnumerable<PlanDto>> GetAllPlansWithDetailsAsync(CancellationToken cancellationToken);
        Task<Guid> GetFreePlanPricingIdAsync(CancellationToken cancellationToken);
        Task<List<Guid>> GetPlanFeatureIdsAsync(Guid PlanId, CancellationToken cancellationToken);
        Task<Guid> GetPlanIdAsync(Guid PlanPricingId, CancellationToken cancellationToken);
        Task<Guid> GetVideoStorageFeatureIdAsync(CancellationToken cancellationToken);
        Task<Guid> GetPlanFeatureIdByFeatureIdAsync(Guid PlanId, Guid FeatureId, CancellationToken cancellationToken);
        Task<int?> GetVideoStorageLimitAsync(CancellationToken cancellationToken);
    }
}
