using Application.Features.TenantStudents.Dtos;
using Application.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantStudents.Commands.InviteStudent
{
    internal sealed class InviteStudentCommandHandler : IRequestHandler<InviteStudentCommand, OneOf<StudentResponse, Error>>
    {
        private readonly ICourseInviteRepository _courseInviteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly ICurrentUserId _currentUserId;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InviteStudentCommandHandler(ICourseInviteRepository courseInviteRepository, UserManager<ApplicationUser> userManager,
            ITenantMemberRepository tenantMemberRepository, ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender, ICurrentUserId currentUserId, ICourseRepository courseRepository, IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork)
        {
            _courseInviteRepository = courseInviteRepository;
            _userManager = userManager;
            _tenantMemberRepository = tenantMemberRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _currentUserId = currentUserId;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentResponse, Error>> Handle(InviteStudentCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var currentUserId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(currentUserId, PermissionConstants.MANAGE_STUDENTS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isFeatureEnded = await _tenantRepository.IsFeatureUsingEnded(subDomain!, FeatureConstants.STUDENT_LIMIT, cancellationToken);
            if (isFeatureEnded)
                return TenantErrors.FeatureUsageEnded;

            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var studentUser = await _userManager.FindByEmailAsync(request.StudentEmail);
            if (studentUser is not null)
            {
                var studentId = await _studentRepository.GetStudentIdAsync(studentUser.Id, cancellationToken);
                var isEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(studentId, request.CourseId, cancellationToken);
                if (isEnrolled)
                    return StudentErrors.AlreadyEnrolled;
            }

            var existingInvite = await _courseInviteRepository.GetPendingInviteAsync(request.StudentEmail, request.CourseId, subDomain!, cancellationToken);
            if (existingInvite is not null)
                return CourseInviteErrors.AlreadyInvited;

            var teacher = await _userManager.FindByIdAsync(currentUserId);
            var invitedByMemberId = await _tenantMemberRepository.GetMemberIdByUserIdAsync(currentUserId, tenantId, cancellationToken);
            var tenant = await _tenantRepository.GetLastTenantAsync(subDomain, cancellationToken);
            var courseInvite = new CourseInvite
            {
                Email = request.StudentEmail,
                InvitedBy = invitedByMemberId,
                CourseId = request.CourseId,
                TenantId = tenantId,
            };
            await _courseInviteRepository.CreateCourseInviteAsync(courseInvite, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _tenantRepository.IncreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.STUDENT_LIMIT, cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.CourseInviteTemplate,
                new Dictionary<string, string>
                {
                    { "{{platform_name}}", tenant!.PlatformName },
                    { "{{course_name}}", course.Title },
                    { "{{action_url}}", $"{EmailConstants.CourseInviteUrl}?token={courseInvite.Token}" },
                    { "{{expiry_hours}}", "24" },
                    { "{{inviter_name}}", $"{teacher!.FirstName} {teacher.LastName}" }
                });

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(request.StudentEmail, EmailConstants.Subject, emailBody));
            return new StudentResponse { Message = $"{MessagesConstants.CourseInviteSent} {request.StudentEmail}" };
        }
    }
}