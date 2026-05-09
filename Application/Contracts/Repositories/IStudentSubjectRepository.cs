namespace Application.Contracts.Repositories
{
    public interface IStudentSubjectRepository
    {
        Task CreateStudentSubjectAsync(List<StudentSubject> studentSubjects, CancellationToken cancellationToken);
        Task<Dictionary<string, int>> GetSubjectIdsAsync(List<string> keys, CancellationToken cancellationToken);
        Task<(string, string?)> GetSubjectAndChapterNamesAsync(int subjectId, int? chapterId, int studentId, CancellationToken cancellationToken);
    }
}