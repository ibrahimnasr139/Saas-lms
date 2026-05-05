using Application.Features.StudentLessons.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Queries.GetStudentLessonItem
{
    internal sealed class GetStudentLessonItemQueryHandler : IRequestHandler<GetStudentLessonItemQuery, OneOf<StudentLessonItemDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly ILessonViewRepository _lessonViewRepository;

        public GetStudentLessonItemQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, IStudentSubscriptionRepository studentSubscriptionRepository,
            IModuleItemRepository moduleItemRepository, ILessonViewRepository lessonViewRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _moduleItemRepository = moduleItemRepository;
            _lessonViewRepository = lessonViewRepository;
        }
        public async Task<OneOf<StudentLessonItemDto, Error>> Handle(GetStudentLessonItemQuery request, CancellationToken cancellationToken)
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


            var cacheKey = $"{CacheKeysConstants.CourseKey}_{request.CourseId}_{CacheKeysConstants.ItemKey}_{request.ItemId}";
            var lessonItem = await _hybridCache.GetOrCreateAsync<StudentLessonItemDto?>(
                cacheKey,
                async _ => await _moduleItemRepository.GetStudentLessonItemAsync(request.ItemId, request.CourseId, cancellationToken),
                cancellationToken: cancellationToken
            );

            if (lessonItem is null)
                return ModuleItemErrors.ModuleItemNotFound;

            var isCompleted = await _lessonViewRepository.IsLessonCompletedAsync(session.StudentId, request.ItemId, cancellationToken);
            var conditions = await _moduleItemRepository.GetConditionsStatusAsync(session.StudentId, request.ItemId, cancellationToken);

            ModuleItemStatus status;
            if (isCompleted)
                status = ModuleItemStatus.completed;
            else if (conditions.Any())
                status = conditions.All(x => x) ? ModuleItemStatus.avilable : ModuleItemStatus.locked;
            else
                status = ModuleItemStatus.avilable;

            lessonItem.Status = status;
            lessonItem.IsCompleted = isCompleted;
            return lessonItem;
        }
    }
}