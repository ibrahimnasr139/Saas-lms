using Application.Features.StudentCourse.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentCourse.Queries.GetStudentCourses
{
    internal sealed class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, OneOf<List<StudentCoursesDto>, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;
        private readonly IEnrollmentRepository _enrollmentRepository;
        public GetStudentCoursesQueryHandler(IHttpContextAccessor httpContextAccessor, HybridCache hybridCache,
            IEnrollmentRepository enrollmentRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task<OneOf<List<StudentCoursesDto>, Error>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
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

            var cacheKey = $"{CacheKeysConstants.StudentCoursesKey}_{session.StudentId}";
            return await _hybridCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                return await _enrollmentRepository.GetStudentCoursesAsync(session.StudentId, cancellationToken);
            }, new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1)
            }, cancellationToken: cancellationToken);
        }
    }
}