using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetContentLibraryStatistics
{
    internal class GetContentLibraryStatisticsHandler : IRequestHandler<GetContentLibraryStatisticsQuery, ContentLibraryStatisticsDto>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetContentLibraryStatisticsHandler(ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ContentLibraryStatisticsDto> Handle(GetContentLibraryStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenatId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);
            return await _tenantRepository.GetStatisticsAsync(tenatId, cancellationToken);
        }
    }
}
