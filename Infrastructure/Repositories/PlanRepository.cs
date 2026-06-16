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
        public async Task<IEnumerable<PlanDto>> GetPlansWithDetailsAsync(CancellationToken cancellationToken)
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
        public async Task<(long Used, int Limit)?> GetFeatureUsageInfoAsync(int tenantId, string featureName, CancellationToken cancellationToken)
        {
            var result = await _context.TenantUsage
                .Where(tu => tu.TenantId == tenantId && tu.PlanFeature.Feature.Key == featureName)
                .Select(tu => new
                {
                    tu.Used,
                    Limit = tu.PlanFeature.LimitValue!.Value
                }).FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                return null;
            return (result.Used, result.Limit);
        }
    }
}