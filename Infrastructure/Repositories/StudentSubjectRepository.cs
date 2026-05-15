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
        public async Task<(string, string?)> GetSubjectAndChapterNamesAsync(int subjectId, int? chapterId, int studentId, CancellationToken cancellationToken)
        {
            var result = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId && ss.Id == subjectId)
                .Select(ss => new
                {
                    SubjectName = ss.AvailableSubject.DisplayName,
                    ChapterName = chapterId == null
                        ? null
                        : ss.StudentChapters
                            .Where(sc => sc.Id == chapterId)
                            .Select(sc => sc.Title)
                            .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return (result!.SubjectName, result.ChapterName);
        }
        public async Task<List<SubjectDto>> GetSubjectsAsync(int studentId, CancellationToken cancellationToken)
        {
            return await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}