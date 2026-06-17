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
                    CourseTitle = e.Course.Title,
                    StudentName = e.Student.User.FirstName + " " + e.Student.User.LastName,
                })
                .ToListAsync(cancellationToken);

            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            var tenantId = enrollments.Select(e => e.TenantId).FirstOrDefault();
            var courseIds = enrollments.Select(e => e.CourseId).Distinct().ToList();

            var grades = await _context.StudentGrades
                .Where(sg => sg.TenantId == tenantId && studentIds.Contains(sg.StudentId))
                .Select(sg => new { sg.StudentId, sg.Score, sg.TotalMarks, sg.Type })
                .ToListAsync(cancellationToken);

            var progresses = await _context.CourseProgresses
                .Where(cp => studentIds.Contains(cp.StudentId) && courseIds.Contains(cp.CourseId))
                .Select(cp => new { cp.StudentId, cp.CourseId, cp.CompletedLessons, cp.TotalLessons })
                .ToListAsync(cancellationToken);

            var studentEnrollments = enrollments
                .GroupBy(e => new { e.StudentId, e.StudentName })
                .Select(g => new
                {
                    g.Key.StudentId,
                    g.Key.StudentName,
                    Courses = g.Select(e => e.CourseTitle).Distinct().ToList(),
                    CourseIds = g.Select(e => e.CourseId).Distinct().ToList()
                })
                .ToList();

            return studentEnrollments
                .Select(s =>
                {
                    var studentGrades = grades.Where(g => g.StudentId == s.StudentId).ToList();
                    var quizGrades = studentGrades.Where(g => g.Type == StudentGradeType.Quiz).ToList();
                    var assignmentGrades = studentGrades.Where(g => g.Type == StudentGradeType.Assignment).ToList();

                    var hasQuizzes = quizGrades.Any();
                    var hasAssignments = assignmentGrades.Any();

                    var quizAverage = hasQuizzes
                        ? quizGrades.Average(g => g.TotalMarks > 0 ? g.Score / g.TotalMarks * 100 : 0)
                        : 0;

                    var assignmentAverage = hasAssignments
                        ? assignmentGrades.Average(g => g.TotalMarks > 0 ? g.Score / g.TotalMarks * 100 : 0)
                        : 0;

                    var studentProgresses = progresses
                        .Where(cp => cp.StudentId == s.StudentId && cp.TotalLessons > 0)
                        .ToList();

                    var progressPercentage = studentProgresses.Any()
                        ? studentProgresses.Average(cp => (double)cp.CompletedLessons / cp.TotalLessons * 100)
                        : 0;

                    double quizWeight = hasQuizzes ? 0.5 : 0;
                    double assignmentWeight = hasAssignments ? 0.3 : 0;
                    double progressWeight = 0.2;

                    double totalWeight = quizWeight + assignmentWeight + progressWeight;

                    var overallScore = totalWeight > 0
                        ? (quizAverage * quizWeight +
                           assignmentAverage * assignmentWeight +
                           progressPercentage * progressWeight) / totalWeight
                        : 0;

                    return new TopStudentsPerformanceDto
                    {
                        Id = s.StudentId,
                        Name = s.StudentName,
                        Courses = s.Courses,
                        OverallScore = (int)Math.Round(overallScore),
                        Breakdown = new BreakdownDto
                        {
                            Quizzes = (int)Math.Round(quizAverage),
                            Assignments = (int)Math.Round(assignmentAverage),
                            Progress = (int)Math.Round(progressPercentage)
                        }
                    };
                })
                .OrderByDescending(x => x.OverallScore)
                .Take(5)
                .ToList();
        }
    }
}