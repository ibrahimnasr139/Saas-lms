using Application.Features.TenantStudents.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantStudents.Commands.DeleteStudent
{
    internal sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, OneOf<StudentResponse, Error>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteStudentCommandHandler(IStudentRepository studentRepository, ICurrentUserId currentUserId, ITenantRepository tenantRepository,
            ITenantMemberRepository tenantMemberRepository, ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _currentUserId = currentUserId;
            _tenantRepository = tenantRepository;
            _tenantMemberRepository = tenantMemberRepository;
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<StudentResponse, Error>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var currentUserId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(currentUserId, PermissionConstants.MANAGE_STUDENTS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isFeatureEnded = await _tenantRepository.IsFeatureUsingEnded(subDomain!, FeatureConstants.STUDENT_LIMIT, cancellationToken);
            if (isFeatureEnded)
                return TenantErrors.FeatureUsageEnded;

            var courseExists = await _courseRepository.GetCourseByIdAsync(request.CourseId, subDomain!, cancellationToken);
            if (courseExists is null)
                return CourseErrors.CourseNotFound;

            var result = await _studentRepository.DeleteStudentAsync(request.StudentId, request.CourseId, cancellationToken);
            if (!result)
                return StudentErrors.StudentNotFound;

            await _tenantRepository.DecreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.STUDENT_LIMIT, cancellationToken);
            return new StudentResponse { Message = MessagesConstants.StudentDeleted };
        }
    }
}