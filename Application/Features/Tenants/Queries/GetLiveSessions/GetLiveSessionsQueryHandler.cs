using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLiveSessions
{
    internal sealed class GetLiveSessionsQueryHandler : IRequestHandler<GetLiveSessionsQuery, List<LiveSessionDto>>
    {
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;

        public GetLiveSessionsQueryHandler(ILiveSessionRepository liveSessionRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository)
        {
            _liveSessionRepository = liveSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
        }
        public async Task<List<LiveSessionDto>> Handle(GetLiveSessionsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            return await _liveSessionRepository.GetLiveSessionsByTenantIdAsync(tenantId, cancellationToken);
        }
    }
}
