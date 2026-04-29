using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class AttemptRepository : IAttemptRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public AttemptRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, int quizId, string subdomain, CancellationToken cancellationToken)
        {
            return await _context.QuizAttempts
                .Where(a => a.Id == attemptId && a.ModuleItemId == quizId && a.Quiz.Course.Tenant.SubDomain == subdomain)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<GradeDistribution>> GetAttemptGradeDistributionAsync(int quizId, CancellationToken cancellationToken)
        {
            return await _context.QuizAttempts
                .Where(a => a.ModuleItemId == quizId)
                .GroupBy(a => (a.Score / 10) * 10)
                .Select(g => new GradeDistribution
                {
                    Range = $"{g.Key}-{g.Key + 9}",
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<MostDifficultQuestion>> GetAttemptMostDifficultQuestionsAsync(int quizId, CancellationToken cancellationToken)
        {
            return await _context.Answers
                .Where(a => a.Attempt.ModuleItemId == quizId)
                .GroupBy(a => new { a.QuizQuestionId, a.QuizQuestion.Question.QuestionTitle })
                .Select(g => new MostDifficultQuestion
                {
                    Question = g.Key.QuestionTitle,
                    InCorrectAnswers = g.Count(a => !a.IsCorrect),
                    TotalAnswers = g.Count()
                })
                .OrderByDescending(q => q.InCorrectAnswers)
                .Take(5)
                .ToListAsync(cancellationToken);
        }
        public async Task<AttemptResponse?> GetAttemptResponseByIdAsync(int attemptId, int quizId, string subdomain, CancellationToken cancellationToken)
        {
            return await _context.QuizAttempts
                .Where(a => a.Id == attemptId && a.ModuleItemId == quizId && a.Quiz.Course.Tenant.SubDomain == subdomain)
                .ProjectTo<AttemptResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<AttemptOverTime>> GetAttemptsOverTimeAsync(int quizId, CancellationToken cancellationToken)
        {
            return await _context.QuizAttempts
                .Where(a => a.ModuleItemId == quizId && a.SubmittedAt != null)
                .GroupBy(a => a.SubmittedAt)
                .Select(g => new AttemptOverTime
                {
                    Date = g.Key!.Value,
                    Attempts = g.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
