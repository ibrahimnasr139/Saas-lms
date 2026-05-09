using Application.Features.StudyTools.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Queries.GetStudentQuiz
{
    internal sealed class GetStudentQuizQueryHandler : IRequestHandler<GetStudentQuizQuery, OneOf<StudentQuizDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentQuizRepository _studentQuizRepository;
        public GetStudentQuizQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentQuizRepository studentQuizRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentQuizRepository = studentQuizRepository;
        }
        public async Task<OneOf<StudentQuizDto, Error>> Handle(GetStudentQuizQuery request, CancellationToken cancellationToken)
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

            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(request.QuizId, session.StudentId, cancellationToken);
            if (studentQuiz is null)
                return StudentQuizErrors.NotFound;
            return studentQuiz;
        }
    }
}