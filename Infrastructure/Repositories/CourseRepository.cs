using Application.Features.Courses.Dtos;
using Application.Features.Public.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Infrastructure.Constants;
namespace Infrastructure.Repositories
{
    internal sealed class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public CourseRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> CreateCourse(Course course, CancellationToken cancellationToken)
        {
            await _dbContext.Courses.AddAsync(course);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return course.Id;
        }
        public async Task<AllCoursesDto> GetAllCoursesAsync(string subDomain, string? q, int? gradeId, int? subjectId, string? sortBy, string? sortOrder, CourseStatus? status, int? cursor, string? lastSortValue, CancellationToken cancellationToken)
        {
            var query = _dbContext.Courses.Where(c => c.Tenant.SubDomain == subDomain).AsNoTracking();
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(c => c.Title.Contains(q) || c.Description.Contains(q));
            }
            if (gradeId.HasValue)
            {
                query = query.Where(c => c.GradeId == gradeId.Value);
            }
            if (subjectId.HasValue)
            {
                query = query.Where(c => c.SubjectId == subjectId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(c => c.CourseStatus == status.Value);
            }
            var studentCountQuery = _dbContext.Enrollments.AsNoTracking().Where(e => e.Course.Tenant.SubDomain == subDomain)
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, StudentCount = g.Select(e => e.StudentId).Count().ToString() })
                .DefaultIfEmpty();
            var courseProgressQuery = _dbContext.CourseProgresses.AsNoTracking().Where(p => p.Course.Tenant.SubDomain == subDomain)
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, CompletionRate = g.Where(p => p.TotalLessons > 0).Average(p => (double)p.CompletedLessons / p.TotalLessons).ToString() });
            var lessonsQuery = _dbContext.Lessons.AsNoTracking().Where(l => l.Course.Tenant.SubDomain == subDomain)
                .GroupBy(l => l.CourseId)
                .Select(g => new { CourseId = g.Key, LessonsCount = g.Count().ToString() })
                .DefaultIfEmpty();
            var queryWithCounts = query.LeftJoin(studentCountQuery, c => c.Id, sc => sc.CourseId, (c, sc) => new { Course = c, StudentCount = sc != null ? sc.StudentCount : null! })
                .LeftJoin(courseProgressQuery, c => c.Course.Id, cp => cp.CourseId, (c, cp) => new { c.Course, c.StudentCount, CompletionRate = cp != null ? cp.CompletionRate : null! })
                .LeftJoin(lessonsQuery, c => c.Course.Id, lc => lc.CourseId, (c, lc) => new { c.Course, c.StudentCount, c.CompletionRate, LessonsCount = lc != null ? lc.LessonsCount : null! });
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == SortBy.Date)
                {
                    queryWithCounts = sortOrder == SortDirections.Ascending
                    ? queryWithCounts.OrderBy(c => c.Course.CreatedAt)
                    : queryWithCounts.OrderByDescending(c => c.Course.CreatedAt);
                    if (!string.IsNullOrEmpty(lastSortValue) && DateTime.TryParse(lastSortValue, out var lastDate))
                    {
                        queryWithCounts = sortOrder == SortDirections.Ascending
                            ? queryWithCounts.Where(c => c.Course.CreatedAt >= lastDate)
                            : queryWithCounts.Where(c => c.Course.CreatedAt <= lastDate);
                    }
                }
                else if (sortBy == SortBy.Students)
                {
                    queryWithCounts = sortOrder == SortDirections.Ascending
                    ? queryWithCounts.OrderBy(c => c.StudentCount)
                    : queryWithCounts.OrderByDescending(c => c.StudentCount);
                    if (!string.IsNullOrEmpty(lastSortValue) && int.TryParse(lastSortValue, out var lastCount))
                    {
                        queryWithCounts = sortOrder == SortDirections.Ascending
                            ? queryWithCounts.Where(c => c.StudentCount != null && int.Parse(c.StudentCount) >= lastCount)
                            : queryWithCounts.Where(c => c.StudentCount != null && int.Parse(c.StudentCount) <= lastCount);
                    }
                }
                else if (sortBy == SortBy.Completion)
                {
                    queryWithCounts = sortOrder == SortDirections.Ascending
                    ? queryWithCounts.OrderBy(c => c.CompletionRate)
                    : queryWithCounts.OrderByDescending(c => c.CompletionRate);
                    if (!string.IsNullOrEmpty(lastSortValue) && double.TryParse(lastSortValue, out var lastRate))
                    {
                        queryWithCounts = sortOrder == SortDirections.Ascending
                            ? queryWithCounts.Where(c => c.CompletionRate != null && double.Parse(c.CompletionRate) >= lastRate)
                            : queryWithCounts.Where(c => c.CompletionRate != null && double.Parse(c.CompletionRate) <= lastRate);
                    }
                }
            }
            else
            {
                queryWithCounts = queryWithCounts.OrderBy(c => c.Course.Id);
            }
            var courses = await queryWithCounts
                .Where(c => c.Course.Id > (cursor ?? 0))
                .Take(PaginationLimits.CoursesPageSize + 1)
                .Select(a => new CourseResponseDto
                {
                    Id = a.Course.Id,
                    Title = a.Course.Title,
                    Description = a.Course.Description,
                    Grade = a.Course.Grade.Label,
                    Subject = a.Course.Subject.Label,
                    Status = a.Course.CourseStatus,
                    CreatedAt = a.Course.CreatedAt,
                    ThumbnailUrl = a.Course.ThumbnailUrl,
                    Price = a.Course.Price,
                    StudentsCount = a.StudentCount != null ? int.Parse(a.StudentCount) : 0,
                    CompletionRate = a.CompletionRate != null ? double.Parse(a.CompletionRate) : 0.0,
                    LessonsCount = a.LessonsCount != null ? int.Parse(a.LessonsCount) : 0
                })
                .ToListAsync(cancellationToken);
            var hasMore = courses.Count > PaginationLimits.CoursesPageSize;
            if (hasMore)
            {
                courses.RemoveAt(courses.Count - 1);
            }
            var nextCursor = courses.LastOrDefault()?.Id;
            var lastSort = !string.IsNullOrEmpty(sortBy) && sortBy == SortBy.Date ? courses.LastOrDefault()?.CreatedAt.ToString("o") :
                !string.IsNullOrEmpty(sortBy) && sortBy == SortBy.Students ? courses.LastOrDefault()?.StudentsCount.ToString() :
                !string.IsNullOrEmpty(sortBy) && sortBy == SortBy.Completion ? courses.LastOrDefault()?.CompletionRate.ToString() : null;
            return new AllCoursesDto
            {
                Data = courses,
                HasMore = hasMore,
                NextCursor = nextCursor ?? 0,
                LastSortValue = lastSort
            };
        }
        public async Task<IEnumerable<LookupDto>> GetAllCoursesTitlesAsync(string subDomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Courses
                .Where(c => c.Tenant.SubDomain == subDomain)
                .ProjectTo<LookupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<Course?> GetCourseByIdAsync(int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<StatisticsDto> GetCourseStatisticsAsync(string tenantSubdomain, CancellationToken cancellationToken)
        {
            var response = await _dbContext.Tenants
                .Where(t => t.SubDomain == tenantSubdomain)
                .Select(t => new StatisticsDto
                {
                    TotalCourses = t.Courses.Count(),
                    ActiveCourses = t.Courses.Count(c => c.CourseStatus == CourseStatus.Published),
                    TotalStudentsEnrolled = t.Courses.SelectMany(e => e.Enrollments).Select(x => x.StudentId).Distinct().Count(),

                })
                .FirstOrDefaultAsync(cancellationToken);
            if (response != null)
            {
                response.DraftCourses = response.TotalCourses - response.ActiveCourses;
                var averages = await _dbContext.Courses.Where(c => c.Tenant.SubDomain == tenantSubdomain)
                    .SelectMany(c => c.CourseProgresses).Where(p => p.TotalLessons > 0)
                    .Select(p => (double)p.CompletedLessons / p.TotalLessons).ToListAsync(cancellationToken);
                response.AverageCompletionRate = averages.Count > 0 ? averages.Average() : 0.0;
            }
            return response!;
        }
        public async Task<CourseStatisticsDto?> GetCourseStatisticsByIdAsync(int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Courses.Where(c => c.Id == courseId && c.Tenant.SubDomain == subdomain)
                .ProjectTo<CourseStatisticsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task RemoveCourseAsync(Course course, CancellationToken cancellationToken)
        {
            _dbContext.Courses.Remove(course);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<WebsiteCourseDetailsDto?> GetWebsiteCourseDetailsAsync(int courseId, string subDomain, string? studentUserId, CancellationToken cancellationToken)
        {
            var course = await _dbContext.Courses
                .Where(c => c.Id == courseId && c.Tenant.SubDomain == subDomain && c.CourseStatus == CourseStatus.Published)
                .ProjectTo<WebsiteCourseDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (course is null)
                return null;

            if (!string.IsNullOrEmpty(studentUserId))
            {
                course.IsEnrolled = await _dbContext.Enrollments
                    .AnyAsync(e => e.CourseId == courseId && e.Student.UserId == studentUserId, cancellationToken);
                course.HasPendingOrder = await _dbContext.Orders
                    .AnyAsync(o => o.CourseId == courseId && o.Student.UserId == studentUserId && o.Status == OrderStatus.pending, cancellationToken);
            }
            return course;
        }
        public async Task<Course> GetCourseAsync(int courseId, int tenantId, CancellationToken cancellationToken)
        {
            var course = await _dbContext.Courses.Where(c => c.Id == courseId && c.TenantId == tenantId)
                .FirstOrDefaultAsync(cancellationToken);
            return course!;
        }
        public Task<string> GetCourseNameAsync(int courseId, CancellationToken cancellationToken)
        {
            var result = _dbContext.Courses.Where(c => c.Id == courseId)
                .Select(c => c.Title)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task<Course?> GetCourseWithEnrollmentsAsync(int courseId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task<(string Title, string Level)?> GetCourseTitleAndLevelAsync(int courseId, string subDomain, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Courses
                .Where(c => c.Id == courseId && c.Tenant.SubDomain == subDomain)
                .Select(c => new
                {
                    c.Title,
                    Level = c.Grade.Label
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                return null;

            return (result.Title, result.Level);
        }
    }
}