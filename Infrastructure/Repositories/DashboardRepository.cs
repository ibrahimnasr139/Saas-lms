using Application.Features.Dashboards.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DashboardRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<PendingTaskDto>> GetPendingTasksAsync(string subdomain, CancellationToken cancellationToken)
        {
            var quizTasks = await _context.QuizAttempts
                .Where(qa => qa.GradingStatus != GradingStatus.Graded &&
                    qa.GradingStatus != GradingStatus.Published &&
                    qa.Quiz.Course.Tenant.SubDomain == subdomain &&
                    qa.SubmittedAt != null)
                .GroupBy(qa => new
                {
                    Id = qa.Quiz.ModuleItemId,
                    qa.Quiz.ModuleId,
                    qa.Quiz.CourseId,
                    Title = qa.Quiz.ModuleItem.Title
                })
                .Select(g => new PendingTaskDto
                {
                    Id = g.Key.Id,
                    Title = $"تصحيح {g.Key.Title}",
                    Count = g.Count(),
                    ModuleId = g.Key.ModuleId,
                    CourseId = g.Key.CourseId,
                    LastSubmittedAt = g.Max(x => x.SubmittedAt)
                }).Take(5).ToListAsync(cancellationToken);

            var assignmentTasks = await _context.AssignmentSubmissions
                .Where(aa => aa.Status != AssignmentStatus.Graded &&
                    aa.Assignment.Course.Tenant.SubDomain == subdomain)
                .GroupBy(aa => new
                {
                    Id = aa.AssignmentId,
                    aa.Assignment.ModuleId,
                    aa.Assignment.CourseId,
                    Title = aa.Assignment.ModuleItem.Title
                })
                .Select(g => new PendingTaskDto
                {
                    Id = g.Key.Id,
                    Title = $"تصحيح {g.Key.Title}",
                    Count = g.Count(),
                    ModuleId = g.Key.ModuleId,
                    CourseId = g.Key.CourseId,
                    LastSubmittedAt = g.Max(x => x.SubmittedAt)
                }).Take(5).ToListAsync(cancellationToken);

            return quizTasks
                .Concat(assignmentTasks)
                .OrderByDescending(x => x.LastSubmittedAt)
                .Take(5)
                .ToList();
        }
        public async Task<QuickAnalyticsDto> GetQuickAnalyticsAsync(string subdomain, CancellationToken cancellationToken)
        {
            var totalCourses = await _context.Courses.CountAsync(c => c.Tenant.SubDomain == subdomain, cancellationToken);

            var totalLessons = await _context.Modules
                .Where(m => m.Course.Tenant.SubDomain == subdomain)
                .SumAsync(m => m.ModuleItems.Count(mi => mi.Type == ModuleItemType.Lesson), cancellationToken);

            var newMessages = await _context.DicussionThreads
                .Where(dt => dt.Tenant.SubDomain == subdomain && !dt.DicussionReads.Any(dr => dr.DicussionId == dt.Id))
                .CountAsync(cancellationToken);

            var completionRate = 0;
            var hasProgress = await _context.CourseProgresses
                .AnyAsync(cc => cc.Course.Tenant.SubDomain == subdomain && cc.TotalLessons > 0, cancellationToken);
            if (hasProgress)
            {
                var avg = await _context.CourseProgresses
                    .Where(cc => cc.Course.Tenant.SubDomain == subdomain && cc.TotalLessons > 0)
                    .AverageAsync(cc => (double)cc.CompletedLessons / cc.TotalLessons, cancellationToken);
                completionRate = (int)(avg * 100);
            }

            return new QuickAnalyticsDto
            {
                TotalCourses = totalCourses,
                TotalLessons = totalLessons,
                NewMessages = newMessages,
                CompletionRate = completionRate
            };
        }
        public async Task<List<UpcomingSessionsDto>> GetUpcomingSessionsAsync(string subdomain, CancellationToken cancellationToken)
        {
            return await _context.LiveSessions
                .Where(s => s.Course.Tenant.SubDomain == subdomain && s.Status == LiveSessionStatus.Upcoming)
                .ProjectTo<UpcomingSessionsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<TopStudentsPerformanceDto>> GetTopStudentsPerformanceAsync(string subdomain, CancellationToken cancellationToken)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.Tenant.SubDomain == subdomain)
                .Select(e => new
                {
                    e.StudentId,
                    e.CourseId,
                    e.TenantId,
                    Name = e.Student.User.FirstName + " " + e.Student.User.LastName,
                    Course = e.Course.Title,
                    Grades = e.Student.StudentGrades
                        .Where(sg => sg.TenantId == e.TenantId)
                        .Select(sg => new { sg.Score, sg.Type })
                        .ToList()
                }).ToListAsync(cancellationToken);

            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            var courseIds = enrollments.Select(e => e.CourseId).Distinct().ToList();

            var lessonViews = await _context.LessonViews
                .Where(lv => studentIds.Contains(lv.StudentId) && lv.Lesson.ModuleItem.CourseId != null)
                .Select(lv => new
                {
                    lv.StudentId,
                    CourseId = lv.Lesson.ModuleItem.CourseId,
                    lv.Status
                }).ToListAsync(cancellationToken);

            var totalLessonsPerCourse = await _context.ModuleItems
                .Where(mi => courseIds.Contains(mi.CourseId) && mi.Type == ModuleItemType.Lesson)
                .GroupBy(mi => mi.CourseId)
                .Select(g => new { CourseId = g.Key, Total = g.Count() })
                .ToListAsync(cancellationToken);

            return enrollments
                .Select(e =>
                {
                    var quizGrades = e.Grades.Where(g => g.Type == StudentGradeType.Quiz);
                    var assignmentGrades = e.Grades.Where(g => g.Type == StudentGradeType.Assignment);

                    var quizAverage = quizGrades.Any()
                        ? quizGrades.Average(g => g.Score * 100)
                        : 0;

                    var assignmentAverage = assignmentGrades.Any()
                        ? assignmentGrades.Average(g => g.Score * 100)
                        : 0;

                    var completedLessons = lessonViews
                        .Count(lv => lv.StudentId == e.StudentId && lv.CourseId == e.CourseId && lv.Status == ViewStatus.Completed);

                    var totalLessons = totalLessonsPerCourse
                        .FirstOrDefault(x => x.CourseId == e.CourseId)?.Total ?? 0;

                    var progressPercentage = totalLessons > 0
                        ? (double)completedLessons / totalLessons * 100
                        : 0;

                    return new TopStudentsPerformanceDto
                    {
                        Id = e.StudentId,
                        Name = e.Name,
                        Course = e.Course,
                        Performance = Math.Round(
                            quizAverage * 0.5 +
                            assignmentAverage * 0.3 +
                            progressPercentage * 0.2, 2)
                    };
                })
                .OrderByDescending(x => x.Performance)
                .Take(5)
                .ToList();
        }
    }
}