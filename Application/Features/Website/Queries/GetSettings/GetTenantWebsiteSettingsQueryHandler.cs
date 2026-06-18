using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Queries.GetSettings
{
    internal sealed class GetTenantWebsiteSettingsQueryHandler : IRequestHandler<GetTenantWebsiteSettingsQuery, OneOf<TenantWebsiteSettingsDto, Error>>
    {
        private readonly ITenantWebsiteSettingsRepository _tenantWebsiteSettingsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        public GetTenantWebsiteSettingsQueryHandler(ITenantWebsiteSettingsRepository tenantWebsiteSettingsRepository,
            IHttpContextAccessor httpContextAccessor, ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository)
        {
            _tenantWebsiteSettingsRepository = tenantWebsiteSettingsRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<TenantWebsiteSettingsDto, Error>> Handle(GetTenantWebsiteSettingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_WEBSITE_SETTINGS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;
            return await _tenantWebsiteSettingsRepository.GetSettingsAsync(subDomain!, cancellationToken);
        }
    }
}
