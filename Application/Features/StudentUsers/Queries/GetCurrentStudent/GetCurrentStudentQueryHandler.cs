using Application.Features.StudentUsers.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentUsers.Queries.GetCurrentStudent
{
    internal sealed class GetCurrentStudentQueryHandler : IRequestHandler<GetCurrentStudentQuery, OneOf<CurrentStudentDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentStudentQueryHandler(HybridCache hybridCache, IStudentRepository studentRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hybridCache = hybridCache;
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<CurrentStudentDto, Error>> Handle(GetCurrentStudentQuery request, CancellationToken cancellationToken)
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
            return await _studentRepository.GetCurrentStudentAsync(session.UserId, session.StudentId, cancellationToken);
        }
    }
}