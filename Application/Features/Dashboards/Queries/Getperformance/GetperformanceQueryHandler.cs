using Application.Features.Dashboards.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Dashboards.Queries.Getperformance
{
    internal sealed class GetperformanceQueryHandler : IRequestHandler<GetperformanceQuery, OneOf<DashboardPerformanceDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly HybridCache _hybridCache;
        public GetperformanceQueryHandler(IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IDashboardRepository dashboardRepository, HybridCache hybridCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _subscriptionRepository = subscriptionRepository;
            _dashboardRepository = dashboardRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<DashboardPerformanceDto, Error>> Handle(GetperformanceQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var cacheKey = $"{subdomain}_{CacheKeysConstants.PerformanceKey}";
            await _hybridCache.RemoveAsync(cacheKey, cancellationToken);
            var performanceData = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _dashboardRepository.GetPerformanceAsync(subdomain!, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(6)
                },
                cancellationToken: cancellationToken
            );
            return performanceData;
        }
    }
}