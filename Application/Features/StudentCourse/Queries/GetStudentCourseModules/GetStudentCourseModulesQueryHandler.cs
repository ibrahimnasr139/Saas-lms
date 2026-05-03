using Application.Features.StudentCourse.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentCourse.Queries.GetStudentCourseModules
{
    internal sealed class GetStudentCourseModulesQueryHandler : IRequestHandler<GetStudentCourseModulesQuery, OneOf<List<StudentModuleDto>, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetStudentCourseModulesQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task<OneOf<List<StudentModuleDto>, Error>> Handle(GetStudentCourseModulesQuery request, CancellationToken cancellationToken)
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
            return await _enrollmentRepository.GetStudentCourseModulesAsync(session.StudentId, request.CourseId, cancellationToken);
        }
    }
}