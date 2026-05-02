using Application.Features.TenantWebsite.Dtos;
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
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _tenantWebsiteRepository.GetTenantWebsiteCoursesAsync(subDomain!, request.CourseIds, cancellationToken);
        }
    }
}