using Application.Features.Students.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Queries.GetProfile
{
    internal sealed class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, OneOf<StudentUserProfileDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetProfileQueryHandler(HybridCache hybridCache, IStudentRepository studentUserRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hybridCache = hybridCache;
            _studentRepository = studentUserRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<StudentUserProfileDto, Error>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
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
            return await _studentRepository.GetUserProfileAsync(session.UserId, session.Role, cancellationToken);
        }
    }
}