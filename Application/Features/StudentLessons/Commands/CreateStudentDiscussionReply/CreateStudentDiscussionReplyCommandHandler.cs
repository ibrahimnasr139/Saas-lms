using Application.Contracts.Repositories;
using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Commands.CreateStudentDiscussionReply
{
    internal sealed class CreateStudentDiscussionReplyCommandHandler : IRequestHandler<CreateStudentDiscussionReplyCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateStudentDiscussionReplyCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentSubscriptionRepository studentSubscriptionRepository, IEnrollmentRepository enrollmentRepository,
            IDiscussionRepository discussionRepository, IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _discussionRepository = discussionRepository;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(CreateStudentDiscussionReplyCommand request, CancellationToken cancellationToken)
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
            var newDiscussionReply = new DicussionThreadReply
            {
                Body = request.Content,
                AuthorId = session.UserId,
                DicussionId = request.DiscussionId,
                TenantId = tenantId,
            };
            await _discussionRepository.CreateDiscussionThreadReplyAsync(newDiscussionReply, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionReplyCreated };
        }
    }
}
