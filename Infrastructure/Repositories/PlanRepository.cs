using Application.Features.Plan.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class PlanRepository : IPlanRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PlanRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlanDto>> GetAllPlansWithDetailsAsync(CancellationToken cancellationToken)
        {
            return await _context.Plans
                    .Where(p => p.Slug != "free-trial")
                    .AsNoTracking()
                    .ProjectTo<PlanDto>(_mapper.ConfigurationProvider)
                    .OrderBy(p => p.Slug)
                    .ToListAsync(cancellationToken);
        }
        public async Task<Guid> GetFreePlanPricingIdAsync(CancellationToken cancellationToken)
        {
            var planPricing = await _context.PlanPricings.FirstOrDefaultAsync(p => p.Price == 0);
            return planPricing!.Id;
        }
        public Task<Guid> GetPlanIdAsync(Guid PlanPricingId, CancellationToken cancellationToken)
        {
            return _context.PlanPricings
                    .Where(pp => pp.Id == PlanPricingId)
                    .Select(pp => pp.PlanId)
                    .FirstOrDefaultAsync(cancellationToken);
        }
        public Task<List<Guid>> GetPlanFeatureIdsAsync(Guid PlanId, CancellationToken cancellationToken)
        {
            return _context.PlanFeatures
                    .Where(pf => pf.PlanId == PlanId)
                    .Select(pf => pf.Id)
                    .ToListAsync(cancellationToken);
        }
        public Task<Guid> GetFeatureIdAsync(string featureName, CancellationToken cancellationToken)
        {
            return _context.Features
                    .Where(f => f.Key == featureName)
                    .Select(f => f.Id)
                    .FirstOrDefaultAsync(cancellationToken);
        }
        public Task<Guid> GetPlanFeatureIdByFeatureIdAsync(Guid PlanId, Guid FeatureId, CancellationToken cancellationToken)
        {
            return _context.PlanFeatures
                    .Where(pf => pf.PlanId == PlanId && pf.FeatureId == FeatureId)
                    .Select(pf => pf.Id)
                    .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<int> GetFeatureLimitAsync(Guid planFeatureId, CancellationToken cancellationToken)
        {
            var limit = await _context.PlanFeatures
                    .Where(pf => pf.Id == planFeatureId)
                    .Select(pf => pf.LimitValue)
                    .FirstOrDefaultAsync(cancellationToken);
            return limit.Value;
        }
        public async Task<(int Used, int Limit)?> GetFeatureUsageInfoAsync(int tenantId, string featureName, CancellationToken cancellationToken)
        {
            var result = await _context.TenantUsage
                .Where(tu => tu.TenantId == tenantId && tu.PlanFeature.Feature.Key == featureName)
                .Select(tu => new
                {
                    tu.Used,
                    Limit = tu.PlanFeature.LimitValue!.Value
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                return null;

            return (result.Used, result.Limit);
        }
    }
}