using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class LessonViewRepository : ILessonViewRepository
    {
        private readonly AppDbContext _context;

        public LessonViewRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateLessonViewAsync(LessonView lessonView, CancellationToken cancellationToken)
        {
            await _context.AddAsync(lessonView, cancellationToken);
        }
        public async Task<LessonView?> GetLessonViewAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            return await _context.LessonViews
                     .Include(lv => lv.VideoSegmants)
                     .FirstOrDefaultAsync(lv => lv.StudentId == studentId && lv.ModuleItemId == itemId, cancellationToken);
        }
        public async Task<bool> IsLessonCompletedAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            return await _context.LessonViews.AnyAsync(lv => lv.ModuleItemId == itemId && lv.StudentId == studentId
                && lv.Status == ViewStatus.Completed, cancellationToken);
        }
    }
}