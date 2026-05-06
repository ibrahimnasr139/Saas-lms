using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Commands.UpdateStudentDiscussion
{
    internal sealed class UpdateStudentDiscussionCommandHandler : IRequestHandler<UpdateStudentDiscussionCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentDiscussionCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IModuleItemRepository moduleItemRepository, IStudentSubscriptionRepository studentSubscriptionRepository,
            IEnrollmentRepository enrollmentRepository, IDiscussionRepository discussionRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _discussionRepository = discussionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(UpdateStudentDiscussionCommand request, CancellationToken cancellationToken)
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

            var discussion = await _discussionRepository.GetDicussionThreadAsync(request.DiscussionId, cancellationToken);
            if (discussion is null)
                return DiscussionErrors.DiscussionThreadNotFound;

            var isDiscussionOwner = await _discussionRepository.IsDiscussionOwnerAsync(request.DiscussionId, session.UserId, cancellationToken);
            if (!isDiscussionOwner)
                return DiscussionErrors.NotDiscussionOwner;

            discussion.Content = request.Content;
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionThreadUpdated };
        }
    }
}
