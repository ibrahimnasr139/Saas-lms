using Application.Features.TenantStudents.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Commands.AcceptInvite
{
    internal sealed class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, OneOf<StudentResponse, Error>>
    {
        private readonly ICourseInviteRepository _courseInviteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly HybridCache _hybridCache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AcceptInviteCommandHandler(ICourseInviteRepository courseInviteRepository, IHttpContextAccessor httpContextAccessor,
            IEnrollmentRepository enrollmentRepository, HybridCache hybridCache, UserManager<ApplicationUser> userManager,
            IStudentSubscriptionRepository studentSubscriptionRepository, ICourseRepository courseRepository,
            IModuleRepository moduleRepository, IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork,
            ITenantMemberRepository tenantMemberRepository)
        {
            _courseInviteRepository = courseInviteRepository;
            _httpContextAccessor = httpContextAccessor;
            _enrollmentRepository = enrollmentRepository;
            _hybridCache = hybridCache;
            _userManager = userManager;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _moduleItemRepository = moduleItemRepository;
            _tenantMemberRepository = tenantMemberRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentResponse, Error>> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
        {
            var isValidToken = await _courseInviteRepository.IsValidTokenAsync(request.Token, cancellationToken);
            if (!isValidToken)
                return CourseInviteErrors.InviteExpired;

            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            var studentUser = await _userManager.FindByIdAsync(session.UserId);
            if (studentUser is null)
                return CourseInviteErrors.InviteError;

            var courseInvite = await _courseInviteRepository.GetCourseInviteByTokenAsync(request.Token, cancellationToken);
            if (courseInvite is null)
                return CourseInviteErrors.InviteError;

            if (!string.Equals(studentUser.Email, courseInvite.Email))
                return TenantInviteErrors.InviteError;

            var courseId = courseInvite.CourseId;
            var tenantId = courseInvite.TenantId;
            var course = await _courseRepository.GetCourseAsync(courseId, tenantId, cancellationToken);
            int? firstModuleId = await _moduleRepository.GetFirstModuleIdAsync(courseId, cancellationToken);
            int? firstModuleItemId = await _moduleItemRepository.GetFirstModuleItemAsync(firstModuleId, cancellationToken);
            int? invitedBy = await _tenantMemberRepository.GetTenantmemberIdAsync(tenantId, cancellationToken);

            var newEnrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = session.StudentId,
                EnrollmentType = EnrollmentType.Invited,
                TenantId = tenantId,
                CurrentModuleId = firstModuleId,
                CurrentModuleItemId = firstModuleItemId,
                InvitedBy = invitedBy,
            };
            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            var newSubscription = new StudentSubscription
            {
                StartDate = now,
                EndDate = CalculateEndDate(course, now),
                StudentId = session.StudentId,
                CourseId = courseId,
                TenantId = tenantId
            };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _enrollmentRepository.CreateEnrollmentAsync(newEnrollment, cancellationToken);
                await _studentSubscriptionRepository.CreateSubscriptionAsync(newSubscription, cancellationToken);
                await _courseInviteRepository.AcceptInviteAsync(request.Token, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _hybridCache.RemoveAsync($"{CacheKeysConstants.StudentCoursesKey}_{session.StudentId}", cancellationToken);
                return new StudentResponse { Message = MessagesConstants.CourseInviteAccepted };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return CourseInviteErrors.InviteError;
            }
        }
        private static DateOnly? CalculateEndDate(Course course, DateOnly now)
        {
            return course.PricingType switch
            {
                PricingType.PerSemester => now.AddMonths(3),
                PricingType.Subscription => course.BillingCycle switch
                {
                    BillingCycle.Monthly => now.AddMonths(1),
                    BillingCycle.Annually => now.AddMonths(8),
                    _ => null
                },
                _ => null
            };
        }
    }
}