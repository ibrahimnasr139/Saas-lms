using Application.Features.StudentCourse.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentCourse.Queries.GetStudentCourse
{
    internal sealed class GetStudentCourseQueryHandler : IRequestHandler<GetStudentCourseQuery, OneOf<StudentCourseDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetStudentCourseQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task<OneOf<StudentCourseDto, Error>> Handle(GetStudentCourseQuery request, CancellationToken cancellationToken)
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

            var studentCourse = await _enrollmentRepository.GetStudentCourseAsync(session.StudentId, request.CourseId, cancellationToken);
            if (studentCourse is null)
                return StudentCourseErrors.StudentNotEnrolledInCourse;
            return studentCourse;
        }
    }
}