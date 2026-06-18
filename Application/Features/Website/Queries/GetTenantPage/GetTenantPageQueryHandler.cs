using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Queries.GetTenantPage
{
    internal sealed class GetTenantPageQueryHandler : IRequestHandler<GetTenantPageQuery, OneOf<TenantPageDto, Error>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetTenantPageQueryHandler(ITenantPageRepository tenantWebsiteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<TenantPageDto, Error>> Handle(GetTenantPageQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var result = await _tenantWebsiteRepository.GetTenantPageWithBlockTypeAsync(subDomain!, request.PageId, cancellationToken);
            if (result == null)
                return TenantWebsiteErrors.TenantPageNotFound;
            return result;
        }
    }
}