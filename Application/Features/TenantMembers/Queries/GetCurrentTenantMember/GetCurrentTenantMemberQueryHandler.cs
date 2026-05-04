using Application.Features.TenantMembers.Dtos;

namespace Application.Features.TenantMembers.Queries.GetCurrentTenantMember
{
    internal sealed class GetCurrentTenantMemberQueryHandler : IRequestHandler<GetCurrentTenantMemberQuery, CurrentTenantMemberDto>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly IMapper _mapper;
        private readonly HybridCache _hybridCache;

        public GetCurrentTenantMemberQueryHandler(ICurrentUserId currentUserId,
            ITenantMemberRepository tenantMemberRepository,
            IMapper mapper, HybridCache hybridCache)
        {
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
            _mapper = mapper;
            _hybridCache = hybridCache;
        }
        public async Task<CurrentTenantMemberDto> Handle(GetCurrentTenantMemberQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var cacheKey = $"{CacheKeysConstants.CurrentTenantMemberKey}_{userId}";
            var response = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    var tenantMember = await _tenantMemberRepository.GetCurrentTenantMemberAsync(userId, cancellationToken);
                    var permissions = tenantMember!.HasFullAccess ? null
                        : await _tenantMemberRepository.GetAllPermissions(tenantMember.Id, cancellationToken);
                    tenantMember.permissions = permissions;
                    return tenantMember;
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(1)
                },
                cancellationToken: cancellationToken
            );
            return response;
        }
    }
}
