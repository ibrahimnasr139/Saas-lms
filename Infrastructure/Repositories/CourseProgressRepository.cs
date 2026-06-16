namespace Infrastructure.Repositories
{
    internal sealed class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly AppDbContext _context;

        public CourseProgressRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<CourseProgress?> GetCourseProgressAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return _context.CourseProgresses
                 .FirstOrDefaultAsync(cp => cp.StudentId == studentId && cp.CourseId == courseId, cancellationToken);
        }
    }
}