using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetTenantWebsiteCourses
{
    internal sealed class GetTenantCoursesQueryHandler : IRequestHandler<GetTenantCoursesQuery, List<TenantCourseDto>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetTenantCoursesQueryHandler(ITenantPageRepository tenantWebsiteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantCourseDto>> Handle(GetTenantCoursesQuery request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];
            return await _tenantWebsiteRepository.GetTenantWebsiteCoursesAsync(subDomain!, request.CourseIds, cancellationToken);
        }
    }
}