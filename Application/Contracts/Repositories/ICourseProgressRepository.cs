namespace Application.Contracts.Repositories
{
    public interface ICourseProgressRepository
    {
        public Task<CourseProgress?> GetCourseProgressAsync(int studentId, int courseId, CancellationToken cancellationToken);
    }
}