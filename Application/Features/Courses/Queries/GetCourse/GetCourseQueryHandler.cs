using Application.Features.Courses.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Queries.GetCourse
{
    internal sealed class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, OneOf<SingleCourseDto, Error>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetCourseQueryHandler(ICourseRepository courseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<SingleCourseDto, Error>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subdomain!, cancellationToken);
            if (course is null)
            {
                return CourseErrors.CourseNotFound;
            }
            return _mapper.Map<SingleCourseDto>(course);
        }
    }
}
