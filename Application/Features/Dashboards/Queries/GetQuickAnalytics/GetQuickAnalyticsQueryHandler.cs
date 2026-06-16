using Application.Features.Dashboards.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Dashboards.Queries.GetQuickAnalytics
{
    public sealed class GetQuickAnalyticsQueryHandler : IRequestHandler<GetQuickAnalyticsQuery, OneOf<QuickAnalyticsDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly HybridCache _hybridCache;
        public GetQuickAnalyticsQueryHandler(IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IDashboardRepository dashboardRepository, HybridCache hybridCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _subscriptionRepository = subscriptionRepository;
            _dashboardRepository = dashboardRepository;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<QuickAnalyticsDto, Error>> Handle(GetQuickAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var cacheKey = $"{subdomain}_{CacheKeysConstants.QuickAnalyticsKey}";
            var quickAnalytics = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    return await _dashboardRepository.GetQuickAnalyticsAsync(subdomain!, cancellationToken);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(6)
                },
                cancellationToken: cancellationToken
            );
            return quickAnalytics;
        }
    }
}