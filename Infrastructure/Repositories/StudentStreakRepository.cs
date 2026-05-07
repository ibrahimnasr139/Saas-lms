using Application.Features.StudentUsers.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class StudentStreakRepository : IStudentStreakRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentStreakRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateStudentStreakAsync(StudentStreak studentStreak, CancellationToken cancellationToken)
        {
            await _context.AddAsync(studentStreak, cancellationToken);
        }
        public async Task<StudentStreakDto> GetStudentStreakAsync(int studentId, CancellationToken cancellationToken)
        {
            var streak = await _context.StudentStreaks
                .AsNoTracking()
                .Where(ss => ss.StudentId == studentId)
                .ProjectTo<StudentStreakDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return streak!;
        }
        public async Task<bool> UpdateStudentStreakAsync(int studentId, CancellationToken cancellationToken)
        {
            var streak = await _context.StudentStreaks.FirstOrDefaultAsync(ss => ss.StudentId == studentId, cancellationToken);
            if (streak is null)
                return false;

            var today = DateTime.UtcNow.Date;
            if (streak.LastActivityAt is null)
                streak.CurrentStreak = 1;
            else
            {
                var lastDate = streak.LastActivityAt.Value.Date;
                var diff = (today - lastDate).Days;
                if (diff == 0)
                    return false;
                else if (diff == 1)
                    streak.CurrentStreak++;
                else
                    streak.CurrentStreak = 1;
            }
            if (streak.CurrentStreak > streak.LongestStreak)
                streak.LongestStreak = streak.CurrentStreak;

            streak.LastActivityAt = today;
            return true;
        }
    }
}