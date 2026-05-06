using Application.Features.Lessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Lessons.Queries.GetViews
{
    internal sealed class GetViewsQueryHandler : IRequestHandler<GetViewsQuery, OneOf<IEnumerable<StudentViewsDto>, Error>>
    {

        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILessonRepository _lessonRepository;
        public GetViewsQueryHandler(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
            IModuleRepository moduleRepository, ILessonRepository lessonRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _lessonRepository = lessonRepository;
        }
        public async Task<OneOf<IEnumerable<StudentViewsDto>, Error>> Handle(GetViewsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isLessonFound = await _lessonRepository.IsFound(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (!isLessonFound)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return await _lessonRepository.GetAllStudentsViewsAsync(request.CourseId, request.ItemId, cancellationToken);
        }
    }
}
