using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IAttemptRepository
    {
        Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, int quizId, string subdomain, CancellationToken cancellationToken);
        Task<List<AttemptOverTime>> GetAttemptsOverTimeAsync(int quizId, CancellationToken cancellationToken);
        Task<List<GradeDistribution>> GetAttemptGradeDistributionAsync(int quizId, CancellationToken cancellationToken);
        Task<List<MostDifficultQuestion>> GetAttemptMostDifficultQuestionsAsync(int quizId, CancellationToken cancellationToken);
        Task<AttemptResponse?> GetAttemptResponseByIdAsync(int attemptId, int quizId, string subdomain, CancellationToken cancellationToken);

    }
}
