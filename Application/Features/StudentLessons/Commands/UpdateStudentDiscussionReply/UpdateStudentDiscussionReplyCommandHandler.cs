using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Commands.UpdateStudentDiscussionReply
{
    internal sealed class UpdateStudentDiscussionReplyCommandHandler : IRequestHandler<UpdateStudentDiscussionReplyCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentDiscussionReplyCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentSubscriptionRepository studentSubscriptionRepository, IEnrollmentRepository enrollmentRepository,
            IModuleItemRepository moduleItemRepository, IDiscussionRepository discussionRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _moduleItemRepository = moduleItemRepository;
            _discussionRepository = discussionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(UpdateStudentDiscussionReplyCommand request, CancellationToken cancellationToken)
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

            var discussionReply = await _discussionRepository.GetDicussionThreadReplyAsync(request.ReplyId, cancellationToken);
            if (discussionReply is null)
                return DiscussionErrors.DiscussionReplyNotFound;

            var isDiscussionReplyOwner = await _discussionRepository.IsDiscussionReplyOwnerAsync(request.ReplyId, request.DiscussionId, session.UserId, cancellationToken);
            if (!isDiscussionReplyOwner)
                return DiscussionErrors.NotDiscussionReplyOwner;

            discussionReply.Body = request.Content;
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionReplyUpdated };
        }
    }
}
