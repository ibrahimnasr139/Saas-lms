using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IQuizRepository
    {
        Task<QuizQuestion?> GetQuizQuestion(int quizId, int quizQuestionId, string subdomain, CancellationToken cancellationToken);
        Task RemoveQuizQuestion(QuizQuestion quizQuestion, CancellationToken cancellationToken);
        Task<List<AttemptDto>> GetAttempts(int courseId, int itemId, CancellationToken cancellationToken);
        Task<OverviewDto?> GetQuizOverview(int itemId, CancellationToken cancellationToken);
        Task<QuizMetadata?> GetQuizMetadata(int quizId, int courseId, int moduleId, string subdomain, CancellationToken cancellationToken);
    }
}
