using Application.Features.StudentLessons.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Commands.CreateStudentDiscussion
{
    internal sealed class CreateStudentDiscussionCommandHandler : IRequestHandler<CreateStudentDiscussionCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentDiscussionCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, IStudentSubscriptionRepository studentSubscriptionRepository,
            IModuleItemRepository moduleItemRepository, IDiscussionRepository discussionRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _moduleItemRepository = moduleItemRepository;
            _discussionRepository = discussionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(CreateStudentDiscussionCommand request, CancellationToken cancellationToken)
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

            var tenantId = await _enrollmentRepository.GetTenantIdAsync(session.StudentId, request.CourseId, cancellationToken);
            var moduleId = await _moduleItemRepository.GetModuleIdAsync(request.ItemId, request.CourseId, cancellationToken);
            var newDiscussion = new DicussionThread
            {
                Content = request.Content,
                ItemType = ModuleItemType.Lesson,
                CreatedBy = session.UserId,
                ItemId = request.ItemId,
                TenantId = tenantId,
                CourseId = request.CourseId,
                ModuleId = moduleId
            };
            await _discussionRepository.CreateDiscussionThreadAsync(newDiscussion, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionThreadCreated };
        }
    }
}
