using Application.Features.TenantWebsite.Dtos;

namespace Application.Features.Public.Queries.GetTenantWebsiteCourses
{
    public sealed record GetTenantCoursesQuery(List<int> CourseIds) : IRequest<List<TenantCourseDto>>;
}