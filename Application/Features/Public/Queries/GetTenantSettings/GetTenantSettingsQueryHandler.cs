using Application.Contracts.Repositories;
using Application.Features.TenantWebsiteSettings.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetTenantSettings
{
    internal sealed class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, TenantWebsiteSettingsDto>
    {
        private readonly ITenantWebsiteSettingsRepository _tenantWebsiteSettingsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantSettingsQueryHandler(ITenantWebsiteSettingsRepository tenantWebsiteSettingsRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteSettingsRepository = tenantWebsiteSettingsRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TenantWebsiteSettingsDto> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            return await _tenantWebsiteSettingsRepository.GetSettingsAsync(subDomain, cancellationToken);
        }
    }
}