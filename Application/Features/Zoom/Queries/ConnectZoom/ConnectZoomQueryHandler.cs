using Application.Contracts.Repositories;
using Application.Contracts.Zoom;
using Application.Features.Zoom.Dtos;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Zoom.Queries.ConnectZoom
{
    internal sealed class ConnectZoomQueryHandler : IRequestHandler<ConnectZoomQuery, OneOf<ConnectZoomDto, Error>>
    {
        private readonly IZoomService _zoomService;
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly IZoomOAuthStateRepository _zoomOAuthStateRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConnectZoomQueryHandler(IZoomService zoomService, ICurrentUserId currentUserId,IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, IZoomOAuthStateRepository zoomOAuthStateRepository, ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository, IUnitOfWork unitOfWork)
        {
            _zoomService = zoomService;
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _zoomOAuthStateRepository = zoomOAuthStateRepository;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<ConnectZoomDto, Error>> Handle(ConnectZoomQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var hasFeature = await _subscriptionRepository.TenantHasFeatureAsync(tenantId, LiveSessionConstants.LiveSessionFeatureKey, cancellationToken);
            if (!hasFeature)
                return LiveSessionErrors.ZoomIntegrationNotAvailable;

            await _zoomOAuthStateRepository.DeleteAllExpiredAndUsedStatesAsync();

            var stateToken = $"{Guid.NewGuid()}|{subDomain}";
            var oAuthState = new ZoomOAuthState
            {
                StateToken = stateToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                TenantId = tenantId,
                UserId = userId
            };
            await _zoomOAuthStateRepository.CreateAsync(oAuthState, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            string authorizationUrl = _zoomService.GetAuthorizationUrl(stateToken, cancellationToken);
            return new ConnectZoomDto { AuthorizationUrl = authorizationUrl };
        }
    }
}
