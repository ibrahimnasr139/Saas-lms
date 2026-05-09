using Application.Features.StudyTools.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IStudentQuizRepository
    {
        Task CreateStudentQuizAsync(StudentQuiz studentQuiz, CancellationToken cancellationToken);
        Task<StudentQuizDto?> GetStudentQuizAsync(int quizId, int studentId, CancellationToken cancellationToken);
    }
}