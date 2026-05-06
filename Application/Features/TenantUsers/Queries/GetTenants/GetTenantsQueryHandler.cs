using Application.Features.TenantUsers.Dtos;

namespace Application.Features.TenantUsers.Queries.GetTenants
{
    internal sealed class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, IEnumerable<UserTenantsDto>>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantUserRepository _userRepository;
        private readonly HybridCache _hybridCache;

        public GetTenantsQueryHandler(ICurrentUserId currentUserId, ITenantUserRepository userRepository, HybridCache hybridCache)
        {
            _currentUserId = currentUserId;
            _userRepository = userRepository;
            _hybridCache = hybridCache;
        }
        public async Task<IEnumerable<UserTenantsDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var cacheKey = $"{CacheKeysConstants.UserTenantsKey}_{userId}";

            var tenants = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async ct => await _userRepository.GetTenantsAsync(userId, ct),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(1)
                },
                cancellationToken: cancellationToken
            );
            return tenants;
        }
    }
}
