using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsite.Queries.ValidateUrl
{
    internal sealed class ValidateUrlQueryHandler : IRequestHandler<ValidateUrlQuery, ValidateUrlDto>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateUrlQueryHandler(ITenantPageRepository tenantWebsiteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ValidateUrlDto> Handle(ValidateUrlQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];

            string url = request.Url?.Trim() ?? "/";
            url = url.StartsWith("/") ? url : $"/{url}";
            return new ValidateUrlDto(await _tenantWebsiteRepository.UrlExistsAsync(subDomain!, url, cancellationToken));
        }
    }
}