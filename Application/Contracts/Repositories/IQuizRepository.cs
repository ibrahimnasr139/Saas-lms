using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;
using Application.Features.StudentQuizes.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IQuizRepository
    {
        Task<QuizQuestion?> GetQuizQuestion(int quizId, int quizQuestionId, string subdomain, CancellationToken cancellationToken);
        Task RemoveQuizQuestion(QuizQuestion quizQuestion, CancellationToken cancellationToken);
        Task<List<AttemptDto>> GetAttempts(int courseId, int itemId, CancellationToken cancellationToken);
        Task<OverviewDto?> GetQuizOverview(int itemId, CancellationToken cancellationToken);
        Task<QuizMetadata?> GetQuizMetadata(int quizId, int courseId, int moduleId, string subdomain, CancellationToken cancellationToken);
        Task<StudentQuizDto?> GetStudentQuizAsync(int studentId, int courseId, int itemId, CancellationToken cancellationToken);
        Task CreateQuizAttemptAsync(QuizAttempt quizAttempt, CancellationToken cancellationToken);
        Task<Quiz> GetQuizAsync(int quizId, CancellationToken cancellationToken);
        Task<bool> IsStudentAttemptedAsync(int studentId, int quizId, CancellationToken cancellationToken);
        Task<QuizAttempt?> GetStudentAttemptedAsync(int studentId, int quizId, CancellationToken cancellationToken);
        Task<Dictionary<int, (int, string, int, int)>> GetQuestionIdsAsync(int quizId, List<int> quizQuestionIds, List<string> answerValues, int attemptId, CancellationToken cancellationToken);
        Task<int> GradeQuizAttemptAsync(Dictionary<int, (int, string, int, int)> questionIdsWithAnswers, CancellationToken cancellationToken);
        Task UpdateQuizAttempt(QuizAttempt quizAttempt);
        Task<List<QuizDeadlineReminderDto>> GetQuizzesEndingWithin24HoursAsync(CancellationToken cancellationToken);
    }
}