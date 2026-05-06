using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsite.Queries.ValidateUrl
{
    internal sealed class ValidateUrlQueryHandler : IRequestHandler<ValidateUrlQuery, ValidateUrlDto>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateUrlQueryHandler(ITenantPageRepository tenantWebsiteRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ValidateUrlDto> Handle(ValidateUrlQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            string url = request.Url?.Trim() ?? "/";
            url = url.StartsWith("/") ? url : $"/{url}";
            return new ValidateUrlDto(await _tenantWebsiteRepository.UrlExistsAsync(tenantId, url, cancellationToken));
        }
    }
}