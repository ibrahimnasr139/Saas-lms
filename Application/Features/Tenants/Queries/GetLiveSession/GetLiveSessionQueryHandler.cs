using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLiveSession
{
    internal sealed class GetLiveSessionQueryHandler : IRequestHandler<GetLiveSessionQuery, LiveSessionDto>
    {
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;

        public GetLiveSessionQueryHandler(ILiveSessionRepository liveSessionRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository)
        {
            _liveSessionRepository = liveSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
        }
        public async Task<LiveSessionDto> Handle(GetLiveSessionQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            return await _liveSessionRepository.GetLiveSessionBySessionIdAsync(request.SessionId, tenantId, cancellationToken);
        }
    }
}
