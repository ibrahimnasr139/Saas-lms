using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Queries.GetStudentLessonTranscript
{
    internal sealed class GetStudentLessonTranscriptQueryHandler : IRequestHandler<GetStudentLessonTranscriptQuery, OneOf<StudentLessonTranscriptDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly ILessonRepository _lessonRepository;
        public GetStudentLessonTranscriptQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, IStudentSubscriptionRepository studentSubscriptionRepository,
            IModuleItemRepository moduleItemRepository, ILessonRepository lessonRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _moduleItemRepository = moduleItemRepository;
            _lessonRepository = lessonRepository;
        }
        public async Task<OneOf<StudentLessonTranscriptDto, Error>> Handle(GetStudentLessonTranscriptQuery request, CancellationToken cancellationToken)
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

            var moduleItemIsExist = await _moduleItemRepository.ModuleItemIsExistAsync(request.ItemId, request.CourseId, cancellationToken);
            if (!moduleItemIsExist)
                return ModuleItemErrors.ModuleItemNotFound;

            var videoId = await _lessonRepository.GetVideoIdAsync(request.ItemId, request.CourseId, cancellationToken);
            return await _lessonRepository.GetStudentLessonTranscriptAsync(videoId, cancellationToken)
                ?? new StudentLessonTranscriptDto();
        }
    }
}