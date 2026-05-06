using Application.Features.Friends.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Friends.Queries.GetFriends
{
    internal sealed class GetStudentFriendsQueryHandler : IRequestHandler<GetStudentFriendsQuery, OneOf<List<FriendsDto>, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFriendRepository _friendRepository;

        public GetStudentFriendsQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IFriendRepository friendRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _friendRepository = friendRepository;
        }
        public async Task<OneOf<List<FriendsDto>, Error>> Handle(GetStudentFriendsQuery request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;
            return await _friendRepository.GetFriendsAsync(session.StudentId, cancellationToken);
        }
    }
}