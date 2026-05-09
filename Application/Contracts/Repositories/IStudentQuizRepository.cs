using Application.Features.StudyTools.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IStudentQuizRepository
    {
        Task CreateStudentQuizAsync(StudentQuiz studentQuiz, CancellationToken cancellationToken);
        Task CreateStudentQuizAttemptAsync(StudentQuizAttempt studentQuizAttempt, CancellationToken cancellationToken);
        Task<StudentQuizDto?> GetStudentQuizAsync(int quizId, int studentId, CancellationToken cancellationToken);
        Task<bool> StudentQuizIsExistAsync(int quizId, int studentId, CancellationToken cancellationToken);
        Task<List<string>> GetSelectedOptionsTextAsync(int quizId, int studentId, List<(int QuestionId, int OptionIndex)> answers, CancellationToken cancellationToken);
    }
}