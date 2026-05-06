using Application.Features.TenantWebsiteSettings.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsiteSettings.Commands.UpdateTenantWebsiteSettings
{
    internal sealed class UpdateTenantWebsiteSettingsCommandHandler : IRequestHandler<UpdateTenantWebsiteSettingsCommand, OneOf<TenantWebsiteSettingsResponse, Error>>
    {
        private readonly ITenantWebsiteSettingsRepository _tenantWebsiteSettingsRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        public UpdateTenantWebsiteSettingsCommandHandler(ITenantWebsiteSettingsRepository tenantWebsiteSettingsRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository)
        {
            _tenantWebsiteSettingsRepository = tenantWebsiteSettingsRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<TenantWebsiteSettingsResponse, Error>> Handle(UpdateTenantWebsiteSettingsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_WEBSITE_SETTINGS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isUpdated = await _tenantWebsiteSettingsRepository.UpdateSettingsAsync(tenantId, request, cancellationToken);
            if (!isUpdated)
                return TenantWebsiteSettingsErrors.TenantWebsiteSettingsUpdateFailed;
            else
                return new TenantWebsiteSettingsResponse { Message = MessagesConstants.TenantWebsiteSettingsUpdated };
        }
    }
}
