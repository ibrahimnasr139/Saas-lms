using Application.Features.StudentCourse.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentCourse.Queries.GetStudentCourseLiveSessions
{
    internal sealed class GetStudentCourseLiveSessionsQueryHandler : IRequestHandler<GetStudentCourseLiveSessionsQuery, OneOf<List<StudentCourseLiveSessionsDto>, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILiveSessionRepository _liveSessionRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;

        public GetStudentCourseLiveSessionsQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, ILiveSessionRepository liveSessionRepository,
            IStudentSubscriptionRepository studentSubscriptionRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _liveSessionRepository = liveSessionRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
        }
        public async Task<OneOf<List<StudentCourseLiveSessionsDto>, Error>> Handle(GetStudentCourseLiveSessionsQuery request, CancellationToken cancellationToken)
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

            var isEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!isEnrolled)
                return StudentCourseErrors.StudentNotEnrolledInCourse;

            var subscriptionIsActive = await _studentSubscriptionRepository.StudentSubscriptionIsActiveAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!subscriptionIsActive)
                return StudentSubscriptionErrors.StudentSubscribedExpired;

            return await _liveSessionRepository.GetStudentCourseLiveSessionsAsync(request.CourseId, cancellationToken);
        }
    }
}