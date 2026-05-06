using Application.Features.Lessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Lessons.Queries.GetLessonOverview
{
    internal sealed class GetLessonOverviewQueryHandler : IRequestHandler<GetLessonOverviewQuery, OneOf<LessonOverviewDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILessonRepository _lessonRepository;
        public GetLessonOverviewQueryHandler(IHttpContextAccessor httpContextAccessor, ILessonRepository lessonRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _lessonRepository = lessonRepository;
        }
        public async Task<OneOf<LessonOverviewDto, Error>> Handle(GetLessonOverviewQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isLessonFound = await _lessonRepository.IsFound(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (!isLessonFound)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            var overview = await _lessonRepository.GetLessonOverviewAsync(request.ItemId, cancellationToken);
            var peakActivity = await _lessonRepository.GetPeakActivityTimeAsync(request.ItemId, cancellationToken);
            overview?.PeakActivity = peakActivity;
            return overview!;
        }
    }
}
