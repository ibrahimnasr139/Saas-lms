using Application.Features.Students.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Queries.GetSubjects
{
    internal sealed class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, OneOf<List<SubjectDto>, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubjectRepository _studentSubjectRepository;

        public GetSubjectsQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, 
            IStudentSubjectRepository studentSubjectRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubjectRepository = studentSubjectRepository;
        }
        public async Task<OneOf<List<SubjectDto>, Error>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
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

            return await _studentSubjectRepository.GetSubjectsAsync(session.StudentId, cancellationToken);
        }
    }
}