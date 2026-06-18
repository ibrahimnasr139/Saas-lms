using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Queries.GetTenantPages
{
    internal sealed class GetTenantPagesQueryHandler : IRequestHandler<GetTenantPagesQuery, List<TenantPagesDto>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantPagesQueryHandler(ITenantPageRepository tenantWebsiteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantPagesDto>> Handle(GetTenantPagesQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _tenantWebsiteRepository.GetTenantPagesAsync(subDomain!, cancellationToken);
        }
    }
}
