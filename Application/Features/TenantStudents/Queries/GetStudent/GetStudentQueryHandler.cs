using Application.Features.TenantStudents.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantStudents.Queries.GetStudent
{
    internal sealed class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, OneOf<StudentDto, Error>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetStudentQueryHandler(IStudentRepository studentRepository, ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<StudentDto, Error>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var result = await _studentRepository.GetTenantStudentAsync(request.StudentId, subDomain!, cancellationToken);
            if (result is null)
                return StudentErrors.StudentNotFound;
            return result;
        }
    }
}