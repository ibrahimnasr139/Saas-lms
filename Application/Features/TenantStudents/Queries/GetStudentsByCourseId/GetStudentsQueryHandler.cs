using Application.Features.TenantStudents.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantStudents.Queries.GetStudentsByCourseId
{
    public sealed class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, OneOf<List<StudentsDto>, Error>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetStudentsQueryHandler(IStudentRepository studentRepository, ICourseRepository courseRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<List<StudentsDto>, Error>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            if (request.CourseId.HasValue)
            {
                var courseExists = await _courseRepository.GetCourseByIdAsync(request.CourseId.Value, subDomain!, cancellationToken);
                if (courseExists is null)
                    return CourseErrors.CourseNotFound;
                return await _studentRepository.GetStudentsAsync(subDomain!, cancellationToken, request.CourseId);
            }
            return await _studentRepository.GetStudentsAsync(subDomain!, cancellationToken);
        }
    }
}