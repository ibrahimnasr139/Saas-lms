using Application.Features.Dashboards.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Dashboards.Queries.GetStatistics
{
    internal sealed class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, OneOf<DashboardStatisticsDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly HybridCache _hybridCache;
        public GetStatisticsQueryHandler(IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IDashboardRepository dashboardRepository, HybridCache hybridCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _subscriptionRepository = subscriptionRepository;
            _dashboardRepository = dashboardRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<DashboardStatisticsDto, Error>> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var cacheKey = $"{subdomain}_{CacheKeysConstants.DashboardStatisticsKey}";
            var statisticsData = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _dashboardRepository.GetStatisticsAsync(subdomain!, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(6)
                },
                cancellationToken: cancellationToken
            );
            return statisticsData;
        }
    }
}