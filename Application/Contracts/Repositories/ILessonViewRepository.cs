namespace Application.Contracts.Repositories
{
    public interface ILessonViewRepository
    {
        Task CreateLessonViewAsync(LessonView lessonView, CancellationToken cancellationToken);
        Task<LessonView?> GetLessonViewAsync(int studentId, int itemId, CancellationToken cancellationToken);
        Task<bool> IsLessonCompletedAsync(int studentId, int itemId, CancellationToken cancellationToken);
    }
}