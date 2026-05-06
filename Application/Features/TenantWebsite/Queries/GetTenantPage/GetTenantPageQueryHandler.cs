using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsite.Queries.GetTenantPage
{
    internal sealed class GetTenantPageQueryHandler : IRequestHandler<GetTenantPageQuery, OneOf<TenantPageDto, Error>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetTenantPageQueryHandler(ITenantPageRepository tenantWebsiteRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<TenantPageDto, Error>> Handle(GetTenantPageQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var result = await _tenantWebsiteRepository.GetTenantPageWithBlockTypeAsync(tenantId, request.PageId, cancellationToken);
            if (result == null)
                return TenantWebsiteErrors.TenantPageNotFound;

            return result;
        }
    }
}
