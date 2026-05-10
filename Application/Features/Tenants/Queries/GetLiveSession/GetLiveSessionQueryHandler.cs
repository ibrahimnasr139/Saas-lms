using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Queries.GetLiveSession
{
    internal sealed class GetLiveSessionQueryHandler : IRequestHandler<GetLiveSessionQuery, LiveSessionDto>
    {
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetLiveSessionQueryHandler(ILiveSessionRepository liveSessionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _liveSessionRepository = liveSessionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<LiveSessionDto> Handle(GetLiveSessionQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _liveSessionRepository.GetLiveSessionBySessionIdAsync(request.SessionId, subDomain!, cancellationToken);
        }
    }
}
