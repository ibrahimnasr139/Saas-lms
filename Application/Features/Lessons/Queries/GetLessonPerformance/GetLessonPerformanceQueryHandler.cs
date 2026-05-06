using Application.Features.Lessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Lessons.Queries.GetLessonPerformance
{
    internal sealed class GetLessonPerformanceQueryHandler : IRequestHandler<GetLessonPerformanceQuery, OneOf<LessonPerformanceDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILessonRepository _lessonRepository;
        public GetLessonPerformanceQueryHandler(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
            IModuleRepository moduleRepository, ILessonRepository lessonRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _lessonRepository = lessonRepository;
        }
        public async Task<OneOf<LessonPerformanceDto, Error>> Handle(GetLessonPerformanceQuery request, CancellationToken cancellationToken)
        {

            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isLessonFound = await _lessonRepository.IsFound(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (!isLessonFound)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            var views = await _lessonRepository.GetViewsOverTimeAsync(request.ItemId, cancellationToken);
            return new LessonPerformanceDto
            {
                ViewsOverTime = views
            };
        }
    }
}
