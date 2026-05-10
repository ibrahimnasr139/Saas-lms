using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLiveSessions
{
    internal sealed class GetLiveSessionsQueryHandler : IRequestHandler<GetLiveSessionsQuery, List<LiveSessionDto>>
    {
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetLiveSessionsQueryHandler(ILiveSessionRepository liveSessionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _liveSessionRepository = liveSessionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<LiveSessionDto>> Handle(GetLiveSessionsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _liveSessionRepository.GetLiveSessionsAsync(subDomain!, cancellationToken);
        }
    }
}
