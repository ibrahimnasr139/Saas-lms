using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetContentLibraryResources
{
    internal class GetContentLibraryResourcesQueryHandler : IRequestHandler<GetContentLibraryResourcesQuery, ContentLibraryResourceDto>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetContentLibraryResourcesQueryHandler(ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ContentLibraryResourceDto> Handle(GetContentLibraryResourcesQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _tenantRepository.GetTenantLibraryResource(subdomain!, request.Type, request.Q, cancellationToken);
        }
    }
}