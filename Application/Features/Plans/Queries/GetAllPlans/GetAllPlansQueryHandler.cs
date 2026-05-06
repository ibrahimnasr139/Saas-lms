using Application.Features.Plans.Dtos;

namespace Application.Features.Plans.Queries.GetAllPlans
{
    public sealed class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, PlansResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IPlanRepository _planRepository;
        private readonly HybridCache _hybridCache;

        public GetAllPlansQueryHandler(IMapper mapper, IPlanRepository planRepository, HybridCache hybridCache)
        {
            _mapper = mapper;
            _planRepository = planRepository;
            _hybridCache = hybridCache;
        }

        public async Task<PlansResponseDto> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeysConstants.PlanKey;
            var plans = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    var data = await _planRepository.GetAllPlansWithDetailsAsync(cancellationToken);
                    return new PlansResponseDto { Plans = data.ToList() };
                },
                cancellationToken: cancellationToken
            );
            return plans;
        }
    }
}
