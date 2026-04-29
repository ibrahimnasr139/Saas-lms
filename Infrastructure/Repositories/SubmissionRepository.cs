using Application.Features.Assignments.Dtos;

namespace Infrastructure.Repositories
{
    internal sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly AppDbContext _context;
        public SubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GradeDistribution>> GetSubmissionGradeDistributionAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _context.AssignmentSubmissions.Where(s => s.AssignmentId == itemId)
                   .GroupBy(g => (g.EarnedMarks / 10) * 10)
                   .Select(g => new GradeDistribution
                   {
                       Range = $"{g.Key}-{g.Key + 9}",
                       Count = g.Count()
                   })
                   .ToListAsync(cancellationToken);
        }
        public async Task<List<SubmissionOverTime>> GetSubmissionsOverTimeAsync(int itemId, CancellationToken cancellationToken)
        {
            return await _context.AssignmentSubmissions.Where(s => s.AssignmentId == itemId)
                .GroupBy(s => s.SubmittedAt.Date)
                .Select(g => new SubmissionOverTime
                {
                    Date = g.Key,
                    Submissions = g.Count()
                })
                .ToListAsync(cancellationToken);
        }
        public async Task GradeSubmission(int submissionId, double grade, string? feedback, CancellationToken cancellationToken)
        {
            await _context.AssignmentSubmissions.Where(s => s.Id == submissionId && s.Assignment.Marks >= grade)
                .ExecuteUpdateAsync(s => s.SetProperty(sub => sub.EarnedMarks, grade)
                    .SetProperty(sub => sub.Feedback, feedback), cancellationToken);
        }
        public async Task<bool> IsSubmissionFound(int submissionId, int itemId, CancellationToken cancellationToken)
        {
            return await _context.AssignmentSubmissions
                .AnyAsync(s => s.Id == submissionId && s.AssignmentId == itemId, cancellationToken);
        }
    }
}
