using Application.Features.Plans.Dtos;

namespace Application.Features.Plans.Queries.GetAllPlan
{
    public sealed class GetAllPlanQueryHandler : IRequestHandler<GetAllPlanQuery, PlansDto>
    {
        private readonly IPlanRepository _planRepository;
        private readonly HybridCache _hybridCache;

        public GetAllPlanQueryHandler(IPlanRepository planRepository, HybridCache hybridCache)
        {
            _planRepository = planRepository;
            _hybridCache = hybridCache;
        }

        public async Task<PlansDto> Handle(GetAllPlanQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeysConstants.PlanKey;
            var plans = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    var data = await _planRepository.GetPlansWithDetailsAsync(cancellationToken);
                    return new PlansDto { Plans = data.ToList() };
                },
                cancellationToken: cancellationToken
            );
            return plans;
        }
    }
}