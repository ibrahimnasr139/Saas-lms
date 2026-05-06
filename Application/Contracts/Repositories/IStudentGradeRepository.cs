namespace Application.Contracts.Repositories
{
    public interface IStudentGradeRepository
    {
        Task<StudentGrade?> GetStudentGradeAsync(int studentId, int itemId, CancellationToken cancellationToken);
        Task CreateStudentGradeAsync(StudentGrade studentGrade, CancellationToken cancellationToken);
    }
}