using Application.Features.Public.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetTenantNavigationLinks
{
    internal sealed class GetTenantNavigationLinksQueryHandler : IRequestHandler<GetTenantNavigationLinksQuery, List<TenantNavigationLinkDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantPageRepository _tenantPageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantNavigationLinksQueryHandler(ITenantRepository tenantRepository, ITenantPageRepository tenantPageRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _tenantPageRepository = tenantPageRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantNavigationLinkDto>> Handle(GetTenantNavigationLinksQuery request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain, cancellationToken);
            return await _tenantPageRepository.GetTenantNavigationLinksAsync(tenantId, cancellationToken);
        }
    }
}