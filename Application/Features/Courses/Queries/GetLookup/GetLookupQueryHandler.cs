using Application.Features.Courses.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Courses.Queries.GetLookup
{
    internal sealed class GetLookupQueryHandler : IRequestHandler<GetLookupQuery, IEnumerable<LookupDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetLookupQueryHandler(ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor)
        {
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<LookupDto>> Handle(GetLookupQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _courseRepository.GetAllCoursesTitlesAsync(subdomain!, cancellationToken);
        }
    }
}
