using Application.Features.Tenants.Dtos;
using Domain.Enums;
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
            var tenantId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);

            FileType type = 0;
            if (!string.IsNullOrWhiteSpace(request.type) && Enum.TryParse<FileType>(request.type, true, out var parsedType))
                type = parsedType;
            return await _tenantRepository.GetTenantLibraryResource(tenantId, type, request.q, cancellationToken);
        }
    }
}
