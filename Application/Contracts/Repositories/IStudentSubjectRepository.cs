using Application.Features.Students.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IStudentSubjectRepository
    {
        Task CreateStudentSubjectAsync(List<StudentSubject> studentSubjects, CancellationToken cancellationToken);
        Task<Dictionary<string, int>> GetSubjectIdsAsync(List<string> keys, CancellationToken cancellationToken);
        Task<List<SubjectDto>> GetSubjectsAsync(int studentId, CancellationToken cancellationToken);
        Task<string?> GetSubjectNameAsync(int studentId, int subjectId, CancellationToken cancellationToken);
        Task<int> GetAvailableSubjectIdAsync(int subjectId, int studentId, CancellationToken cancellationToken);
        Task<string?> GetChapterNameAsync(int availableSubjectId, int chapterId, CancellationToken cancellationToken);
    }
}