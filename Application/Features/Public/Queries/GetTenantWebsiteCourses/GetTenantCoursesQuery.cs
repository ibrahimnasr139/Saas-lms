using Application.Features.Website.Dtos;

namespace Application.Features.Public.Queries.GetTenantWebsiteCourses
{
    public sealed record GetTenantCoursesQuery(List<int> CourseIds) : IRequest<List<TenantCourseDto>>;
}