using Application.Features.Zoom.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Zoom.Queries.GetZoomStatus
{
    internal sealed class GetZoomStatusQueryHandler : IRequestHandler<GetZoomStatusQuery, GetZoomStatusResponse>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IZoomIntegrationRepository _zoomIntegrationRepository;
        private readonly ITenantRepository _tenantRepository;

        public GetZoomStatusQueryHandler(ICurrentUserId currentUserId, IHttpContextAccessor httpContextAccessor,
            IZoomIntegrationRepository zoomIntegrationRepository, ITenantRepository tenantRepository)
        {
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _zoomIntegrationRepository = zoomIntegrationRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<GetZoomStatusResponse> Handle(GetZoomStatusQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var zoomIntegration = await _zoomIntegrationRepository.GetZoomIntegrationAsync(userId, tenantId, cancellationToken);

            if (zoomIntegration == null)
            {
                return new GetZoomStatusResponse
                {
                    IsConnected = false,
                    AccountInfo = null
                };
            }
            return new GetZoomStatusResponse
            {
                IsConnected = true,
                AccountInfo = new ZoomAccountInfo
                {
                    ZoomUserId = zoomIntegration.ZoomUserId,
                    ZoomEmail = zoomIntegration.ZoomEmail,
                    ZoomDisplayName = zoomIntegration.ZoomDisplayName,
                    ZoomAccountType = zoomIntegration.ZoomAccountType,
                    ZoomAccountTypeName = GetAccountTypeName(zoomIntegration.ZoomAccountType),
                    ConnectedAt = zoomIntegration.CreatedAt,
                    LastUsedAt = zoomIntegration.LastSyncAt,
                    IsActive = zoomIntegration.IsActive
                }
            };
        }

        private static string GetAccountTypeName(string accountType)
        {
            return accountType switch
            {
                "1" => "Basic (Free)",
                "2" => "Pro (Licensed)",
                "3" => "Business",
                _ => "Unknown"
            };
        }
    }
}