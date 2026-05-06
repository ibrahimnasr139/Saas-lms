using Application.Features.Roles.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Roles.Queries.GetTenantRole
{
    internal sealed class GetTenantRoleQueryHandler : IRequestHandler<GetTenantRoleQuery, List<TenantRolesDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantRoleQueryHandler(ITenantRepository tenantRepository, ITenantRoleRepository tenantRoleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantRolesDto>> Handle(GetTenantRoleQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            return await _tenantRoleRepository.GetTenantRolesAsync(tenantId, cancellationToken);
        }
    }
}
