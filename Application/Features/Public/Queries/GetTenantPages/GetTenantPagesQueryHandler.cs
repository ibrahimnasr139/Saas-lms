using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetTenantPages
{
    internal sealed class GetTenantPagesQueryHandler : IRequestHandler<GetTenantPagesQuery, OneOf<TenantPageDto, Error>>
    {
        private readonly ITenantPageRepository _tenantPageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTenantPagesQueryHandler(ITenantPageRepository tenantPageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantPageRepository = tenantPageRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<TenantPageDto, Error>> Handle(GetTenantPagesQuery request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var tenantPage = await _tenantPageRepository.GetPublishedTenantPagesAsync(request.Url, subDomain, cancellationToken);
            if (tenantPage is null)
                return TenantWebsiteErrors.TenantPageNotFound;
            return tenantPage;
        }
    }
}
