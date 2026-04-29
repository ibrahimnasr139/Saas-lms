using Application.Features.StudentAssignments.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentAssignments.Commands.SubmitAssignment
{
    internal sealed class SubmitAssignmentCommandHandler : IRequestHandler<SubmitAssignmentCommand, OneOf<StudentAssignmentResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public SubmitAssignmentCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IModuleItemRepository moduleItemRepository, IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork,
            IStudentSubscriptionRepository studentSubscriptionRepository, IAssignmentRepository assignmentRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _assignmentRepository = assignmentRepository;
        }
        public async Task<OneOf<StudentAssignmentResponse, Error>> Handle(SubmitAssignmentCommand request, CancellationToken cancellationToken)
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

            var newAssignmentSubmission = new AssignmentSubmission()
            {
                AssignmentId = request.ItemId,
                StudentId = session.StudentId,
                Status = AssignmentStatus.Submitted,
            };
            if (request.SubmissionType == SubmissionType.File)
                newAssignmentSubmission.FileId = request.FileId;
            else if (request.SubmissionType == SubmissionType.Text)
                newAssignmentSubmission.Text = request.TextContent;
            else
                newAssignmentSubmission.Link = request.Link;

            await _assignmentRepository.CreateAssignmentSubmissionAsync(newAssignmentSubmission, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentAssignmentResponse { SubmissionId = newAssignmentSubmission.Id, Messsage = MessagesConstants.AssignmentSubmissionSuccessfully };
        }
    }
}