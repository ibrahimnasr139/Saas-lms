using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLiveSessionsStatistics
{
    internal sealed class GetLiveSessionsStatisticsQueryHandler : IRequestHandler<GetLiveSessionsStatisticsQuery, OneOf<GetLiveSessionsStatisticsResponse, Error>>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IZoomIntegrationRepository _zoomIntegrationRepository;

        public GetLiveSessionsStatisticsQueryHandler(ICurrentUserId currentUserId, IHttpContextAccessor httpContextAccessor,
           ITenantRepository tenantRepository, ILiveSessionRepository liveSessionRepository, IZoomIntegrationRepository zoomIntegrationRepository)
        {
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _liveSessionRepository = liveSessionRepository;
            _zoomIntegrationRepository = zoomIntegrationRepository;
        }

        public async Task<OneOf<GetLiveSessionsStatisticsResponse, Error>> Handle(GetLiveSessionsStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var zoomIntegration = await _zoomIntegrationRepository.GetZoomIntegrationAsync(userId, tenantId, cancellationToken);
            if (zoomIntegration == null || !zoomIntegration.IsActive)
                return ZoomError.ZoomAccountNotConnected;

            return await _liveSessionRepository.GetStatisticsAsync(userId, tenantId, cancellationToken);
        }
    }
}
