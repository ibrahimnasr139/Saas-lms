using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Queries.GetStatistics
{
    internal sealed class GetWebsiteStatisticsQueryHandler : IRequestHandler<GetWebsiteStatisticsQuery, OneOf<WebSiteStatisticsDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantPageVisitRepository _tenantPageVisitRepository;

        public GetWebsiteStatisticsQueryHandler(IHttpContextAccessor httpContextAccessor, ITenantPageVisitRepository tenantPageVisitRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantPageVisitRepository = tenantPageVisitRepository;
        }
        public async Task<OneOf<WebSiteStatisticsDto, Error>> Handle(GetWebsiteStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _tenantPageVisitRepository.GetTenantStatisticsAsync(subdomain!, cancellationToken);
        }
    }
}