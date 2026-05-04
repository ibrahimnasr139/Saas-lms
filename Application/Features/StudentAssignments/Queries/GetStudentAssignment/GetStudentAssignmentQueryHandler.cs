using Application.Features.StudentAssignments.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentAssignments.Queries.GetStudentAssignment
{
    internal sealed class GetStudentAssignmentQueryHandler : IRequestHandler<GetStudentAssignmentQuery, OneOf<StudentAssignmentDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public GetStudentAssignmentQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentSubscriptionRepository studentSubscriptionRepository, IEnrollmentRepository enrollmentRepository,
            IModuleItemRepository moduleItemRepository, IAssignmentRepository assignmentRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _moduleItemRepository = moduleItemRepository;
            _assignmentRepository = assignmentRepository;
        }
        public async Task<OneOf<StudentAssignmentDto, Error>> Handle(GetStudentAssignmentQuery request, CancellationToken cancellationToken)
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
            var assignment = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async _ =>
                {
                    return await _assignmentRepository.GetAssignmentAsync(request.ItemId, request.CourseId, cancellationToken);
                },
                cancellationToken: cancellationToken,
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(1)
                }
            );
            if (assignment is null)
                return ModuleItemErrors.ModuleItemNotFound;

            var submission = await _assignmentRepository.GetStudentSubmissionAsync(session.StudentId, request.ItemId, cancellationToken);
            var isCompleted = submission != null;
            var conditions = await _assignmentRepository.GetConditionsStatusAsync(session.StudentId, request.ItemId, cancellationToken);
            
            ModuleItemStatus status;
            if (isCompleted)
                status = ModuleItemStatus.completed;

            else if (conditions.Any())
            {
                var allMet = conditions.All(x => x);
                status = allMet ? ModuleItemStatus.avilable : ModuleItemStatus.locked;
            }
            else
                status = ModuleItemStatus.avilable;

            return new StudentAssignmentDto
            {
                Assignment = new AssignmentDto
                {
                    Title = assignment.Title,
                    Description = assignment.Description,
                    Instructions = assignment.Instructions,
                    SubmissionType = assignment.SubmissionType,
                    DueDate = assignment.DueDate,
                    TotalMarks = assignment.TotalMarks,
                    Attachments = assignment.Attachments,
                    CreatedAt = assignment.CreatedAt,
                    UpdatedAt = assignment.UpdatedAt,
                    Status = status
                },
                AssignmentSubmission = submission
            };
        }
    }
}