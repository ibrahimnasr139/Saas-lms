using Application.Features.StudentUsers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentUsers.Queries.GetStudentStreak
{
    internal sealed class GetStudentStreakQueryHandler : IRequestHandler<GetStudentStreakQuery, OneOf<StudentStreakDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentStreakRepository _studentStreakRepository;

        public GetStudentStreakQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentStreakRepository studentStreakRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentStreakRepository = studentStreakRepository;
        }
        public async Task<OneOf<StudentStreakDto, Error>> Handle(GetStudentStreakQuery request, CancellationToken cancellationToken)
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
            return await _studentStreakRepository.GetStudentStreakAsync(session.StudentId, cancellationToken);
        }
    }
}
