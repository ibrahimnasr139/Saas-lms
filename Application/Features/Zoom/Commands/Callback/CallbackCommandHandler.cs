using Application.Contracts.Repositories;
using Application.Contracts.Zoom;

namespace Application.Features.Zoom.Commands.Callback
{
    internal sealed class CallbackCommandHandler : IRequestHandler<CallbackCommand, string>
    {
        private readonly IZoomService _zoomService;
        private readonly IZoomOAuthStateRepository _zoomOAuthStateRepository;
        private readonly IZoomIntegrationRepository _zoomIntegrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CallbackCommandHandler(IZoomService zoomService, IZoomOAuthStateRepository zoomOAuthStateRepository,
            IZoomIntegrationRepository zoomIntegrationRepository, IUnitOfWork unitOfWork)
        {
            _zoomService = zoomService;
            _zoomOAuthStateRepository = zoomOAuthStateRepository;
            _zoomIntegrationRepository = zoomIntegrationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CallbackCommand request, CancellationToken cancellationToken)
        {
            var state = request.state;
            var parts = state.Split('|');
            var subDomain = parts.Length > 1 ? parts[1] : "app";
            var successUrl = $"https://{subDomain}{ZoomConstants.FrontendRedirectUrl}?zoom_connected=true";
            var errorUrl = $"https://{subDomain}{ZoomConstants.FrontendRedirectUrl}?zoom_connected=false";

            var oauthState = await _zoomOAuthStateRepository.TryMarkAsUsedAsync(state, cancellationToken);
            if (oauthState is null)
                return errorUrl;

            var zoomTokenResponse = await _zoomService.ExchangeCodeToTokenAsync(request.code, state, cancellationToken);
            if (zoomTokenResponse is null)
                return errorUrl;

            var zoomUserInfo = await _zoomService.GetZoomUserInfoAsync(zoomTokenResponse.access_token, cancellationToken);
            if (zoomUserInfo is null)
                return errorUrl;

            await _zoomIntegrationRepository.SaveOrUpdateIntegrationAsync(oauthState.UserId, oauthState.TenantId, zoomTokenResponse, zoomUserInfo, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return successUrl;
        }
    }
}