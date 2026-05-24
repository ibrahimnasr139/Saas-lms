using Application.Features.Lessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Lessons.Queries.GetLessonContent
{
    internal sealed class GetLessonContentQueryHandler : IRequestHandler<GetLessonContentQuery, OneOf<LessonContentDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILessonRepository _lessonRepository;
        public GetLessonContentQueryHandler(IHttpContextAccessor httpContextAccessor, ILessonRepository lessonRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _lessonRepository = lessonRepository;
        }
        public async Task<OneOf<LessonContentDto, Error>> Handle(GetLessonContentQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isLessonFound = await _lessonRepository.IsFound(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (!isLessonFound)
                return ModuleItemErrors.ModuleItemNotFound;
            return await _lessonRepository.GetLessonContentAsync(request.CourseId, request.ModuleId, request.ItemId, cancellationToken);
        }
    }
}