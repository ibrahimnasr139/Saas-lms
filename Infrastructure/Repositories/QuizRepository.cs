using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _dbContext;
        public QuizRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AttemptDto>> GetAttempts(int courseId, int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.Students.Where(s => s.Enrollments.Any(c => c.CourseId == courseId))
                 .LeftJoin(_dbContext.QuizAttempts.Where(sv => sv.ModuleItemId == itemId),
                     student => student.Id,
                     quizAttempt => quizAttempt.StudentId,
                     (student, quizAttempt) => new AttemptDto
                     {
                         Id = quizAttempt != null ? quizAttempt.Id : 0,
                         StudentId = student.Id,
                         StudentName = student.User.FirstName + " " + student.User.LastName,
                         ProfilePicture = student.User.ProfilePicture,
                         SubmissionStatus = quizAttempt != null ? quizAttempt.SubmissionStatus : SubmissionStatus.NotStarted,
                         GradingStatus = quizAttempt != null ? quizAttempt.GradingStatus : GradingStatus.NotGraded,
                         SubmittedAt = quizAttempt != null ? quizAttempt.SubmittedAt : null,
                         StartedAt = quizAttempt != null ? quizAttempt.StartedAt : null,
                         TimeSpent = quizAttempt != null ? quizAttempt.TimeSpent : null,
                         Score = quizAttempt != null ? quizAttempt.Score : null,
                         TotalMarks = quizAttempt != null ? quizAttempt.TotalMarks : 0,
                         IsPassed = quizAttempt != null ? quizAttempt.Score >= quizAttempt.Quiz.PassingScore : null
                     }).ToListAsync(cancellationToken);
        }
        public async Task<QuizMetadata?> GetQuizMetadata(int quizId, int courseId, int moduleId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .Where(mi => mi.Id == quizId && mi.ModuleId == moduleId && mi.CourseId == courseId && mi.Course.Tenant.SubDomain == subdomain)
                .Select(mi => new QuizMetadata
                {
                    Course = mi.Course.Title,
                    Module = mi.Module.Title,
                    Title = mi.Title,
                    Description = mi.Description,
                    Subject = mi.Course.Subject.Label,
                    Grade = mi.Course.Grade.Label
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<OverviewDto?> GetQuizOverview(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.QuizAttempts.Where(sv => sv.ModuleItemId == itemId)
                .GroupBy(sv => 1)
                .Select(g => new OverviewDto
                {
                    TotalAttempts = g.Count(),
                    AverageScore = g.Average(a => a.Score) ?? 0,
                    HighestScore = g.Max(a => a.Score) ?? 0,
                    LowestScore = g.Min(a => a.Score) ?? 0,
                    AverageTime = g.Average(a => a.TimeSpent) ?? 0
                }).FirstOrDefaultAsync(cancellationToken);

        }
        public async Task<QuizQuestion?> GetQuizQuestion(int quizId, int quizQuestionId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.QuizQuestions
                .Include(x => x.Question)
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.Id == quizQuestionId && q.Question.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task RemoveQuizQuestion(QuizQuestion quizQuestion, CancellationToken cancellationToken)
        {
            _dbContext.QuizQuestions.Remove(quizQuestion);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
