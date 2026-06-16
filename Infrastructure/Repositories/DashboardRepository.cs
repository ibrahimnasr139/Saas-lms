using Application.Features.Dashboards.Dtos;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;
        public DashboardRepository(AppDbContext context)
        {
            _context = context;
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
                }).ToListAsync(cancellationToken);

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
                }).ToListAsync(cancellationToken);

            return quizTasks
                .Concat(assignmentTasks)
                .OrderByDescending(x => x.LastSubmittedAt)
                .Take(5)
                .ToList();
        }
    }
}