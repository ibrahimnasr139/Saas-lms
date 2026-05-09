namespace Application.Contracts.Repositories
{
    public interface IStudentQuizRepository
    {
        Task CreateStudentQuizAsync(StudentQuiz studentQuiz, CancellationToken cancellationToken);
    }
}