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
    }
}