using Application.Features.Public.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetStatistics
{
    internal sealed class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, OneOf<PublicStatisticsDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantPageVisitRepository _tenantPageVisitRepository;

        public GetStatisticsQueryHandler(IHttpContextAccessor httpContextAccessor, ITenantPageVisitRepository tenantPageVisitRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantPageVisitRepository = tenantPageVisitRepository;
        }
        public async Task<OneOf<PublicStatisticsDto, Error>> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];
            return new PublicStatisticsDto();
        }
    }
}