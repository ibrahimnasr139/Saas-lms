using Application.Features.StudentUsers.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IStudentStreakRepository
    {
        Task CreateStudentStreakAsync(StudentStreak studentStreak, CancellationToken cancellationToken);
        Task<StudentStreakDto> GetStudentStreakAsync(int studentId, CancellationToken cancellationToken);
        Task<bool> UpdateStudentStreakAsync(int studentId, CancellationToken cancellationToken, bool updateActivity = false);
    }
}