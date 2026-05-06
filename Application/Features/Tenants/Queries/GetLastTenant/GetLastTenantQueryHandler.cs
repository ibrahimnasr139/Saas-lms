using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLastTenant
{
    internal sealed class GetLastTenantQueryHandler : IRequestHandler<GetLastTenantQuery, LastTenantDto?>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetLastTenantQueryHandler(ITenantRepository tenantRepository, ICurrentUserId currentUserId, HybridCache hybridCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _currentUserId = currentUserId;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LastTenantDto?> Handle(GetLastTenantQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var cacheKey = $"{CacheKeysConstants.LastTenantKey}_{userId}";

            var tenant = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async ct => await _tenantRepository.GetLastTenantAsync(subdomain, ct),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(1)
                },
                cancellationToken: cancellationToken
            );
            return tenant;
        }
    }
}
