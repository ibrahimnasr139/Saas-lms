using Application.Features.TenantMembers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantMembers.Queries.GetTenantMembers
{
    internal class GetTenantMembersQueryHandler : IRequestHandler<GetTenantMembersQuery, List<TenantMembersDto>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantMembersQueryHandler(ITenantMemberRepository tenantMemberRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantMembersDto>> Handle(GetTenantMembersQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            return await _tenantMemberRepository.GetTenantMembersAsync(tenantId, cancellationToken);
        }
    }
}
