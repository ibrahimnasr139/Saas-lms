using Application.Features.Courses.Dtos;
using Application.Features.Public.Dtos;
using Domain.Enums;

namespace Application.Contracts.Repositories
{
    public interface ICourseRepository
    {
        Task<StatisticsDto> GetCourseStatisticsAsync(string tenantSubdomain, CancellationToken cancellationToken);
        Task<AllCoursesDto> GetAllCoursesAsync(string subDomain, string? q, int? gradeId, int? subjectId, string? sortBy, string? sortOrder, CourseStatus? status,
            int? cursor, string? lastSortValue, CancellationToken cancellationToken);
        Task<IEnumerable<LookupDto>> GetAllCoursesTitlesAsync(string subDomain, CancellationToken cancellationToken);
        Task<int> CreateCourse(Course course, CancellationToken cancellationToken);
        Task<Course?> GetCourseByIdAsync(int courseId, string subdomain, CancellationToken cancellationToken);
        Task<CourseStatisticsDto?> GetCourseStatisticsByIdAsync(int courseId, string subdomain, CancellationToken cancellationToken);
        Task RemoveCourseAsync(Course course, CancellationToken cancellationToken);
        Task<WebsiteCourseDetailsDto?> GetWebsiteCourseDetailsAsync(int courseId, string subDomain, string? studentUserId, CancellationToken cancellationToken);
        Task<Course> GetCourseAsync(int courseId, int tenantId, CancellationToken cancellationToken);
        Task<string> GetCourseNameAsync(int courseId, CancellationToken cancellationToken);
        Task<Course?> GetCourseWithEnrollmentsAsync(int courseId, string subdomain, CancellationToken cancellationToken);
        Task<(string Title, string Level)?> GetCourseTitleAndLevelAsync(int courseId, string subDomain, CancellationToken cancellationToken);
    }
}