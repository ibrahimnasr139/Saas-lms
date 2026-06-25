using Application.Features.Students.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class StudentSubjectRepository : IStudentSubjectRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentSubjectRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateStudentSubjectAsync(List<StudentSubject> studentSubjects, CancellationToken cancellationToken)
        {
            await _context.StudentSubjects.AddRangeAsync(studentSubjects, cancellationToken);
        }
        public async Task<Dictionary<string, int>> GetSubjectIdsAsync(List<string> keys, CancellationToken cancellationToken)
        {
            return await _context.AvailableSubjects
                .Where(s => keys.Contains(s.Key))
                .Select(s => new { s.Key, s.Id })
                .ToDictionaryAsync(s => s.Key, s => s.Id, cancellationToken);
        }
        public async Task<string?> GetChapterNameAsync(int availableSubjectId, int chapterId, CancellationToken cancellationToken)
        {
            return await _context.StudentChapters
                .Where(sc => sc.Id == chapterId && sc.AvailableSubject.Id == availableSubjectId)
                .Select(sc => sc.Title)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<string?> GetSubjectNameAsync(int studentId, int subjectId, CancellationToken cancellationToken)
        {
            return await _context.StudentSubjects
                .Where(ss => ss.Id == subjectId && ss.StudentId == studentId)
                .Select(ss => ss.AvailableSubject.DisplayName)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<SubjectDto>> GetSubjectsAsync(int studentId, CancellationToken cancellationToken)
        {
            var subjects = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var currentChapters = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId && ss.CurrentChaper != null)
                .Select(ss => ss.CurrentChaper!.Value)
                .ToListAsync(cancellationToken);

            foreach (var subject in subjects)
                foreach (var chapter in subject.Chapters)
                    chapter.IsCurrentChapter = currentChapters.Contains(chapter.Id);

            return subjects;
        }
        public async Task<int> GetAvailableSubjectIdAsync(int subjectId, int studentId, CancellationToken cancellationToken)
        {
            return await _context.StudentSubjects
                .Where(ss => ss.Id == subjectId && ss.StudentId == studentId)
                .Select(ss => ss.AvailableSubjectId)
                .FirstAsync(cancellationToken);
        }
    }
}