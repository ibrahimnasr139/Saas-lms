using Application.Contracts.Repositories;
using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Features.StudentLessons.Commands.DeleteStudentDiscussion
{
    internal sealed class DeleteStudentDiscussionCommandHandler : IRequestHandler<DeleteStudentDiscussionCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStudentDiscussionCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
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
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(DeleteStudentDiscussionCommand request, CancellationToken cancellationToken)
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

            await _discussionRepository.DeleteDiscussionThreadAsync(discussion, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionThreadDelete };
        }
    }
}
