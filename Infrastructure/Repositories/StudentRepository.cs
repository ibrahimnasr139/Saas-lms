using Application.Features.Students.Dtos;
using Application.Features.StudentUsers.Dtos;
using Application.Features.TenantStudents.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StudentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<string>> GetStudentsEmails(IEnumerable<int> studentIds, string subdomain, CancellationToken cancellationToken)
        {
            return await _context.Students
                .Where(s => studentIds.Contains(s.Id) && s.User.LastActiveTenantSubDomain == subdomain)
                .Select(s => s.User.Email!)
                .ToListAsync(cancellationToken);
        }
        public Task<Student?> GetStudentAsync(int studentId, CancellationToken cancellationToken)
        {
            return _context.Students
                .Include(s => s.User)
                .Include(s => s.StudentGrades)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }
        public async Task<int> GetStudentIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<StudentsDto>> GetStudentsAsync(string subDomain, CancellationToken cancellationToken, int? courseId = null)
        {
            var studentsQuery = _context.Students
                .AsNoTracking()
                .Where(s => s.Enrollments.Any(e => e.Course.Tenant.SubDomain == subDomain))
                .AsQueryable();

            if (courseId.HasValue)
                studentsQuery = studentsQuery.Where(s => s.Enrollments.Any(e => e.CourseId == courseId.Value));

            var students = await studentsQuery
                .ProjectTo<StudentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return students;
        }
        public async Task<StudentStatisticsDto> GetStudentStatisticsAsync(string subDomain, CancellationToken cancellationToken)
        {
            var studentsCount = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.Course.Tenant.SubDomain == subDomain)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var averageGrade = await _context.StudentGrades
                .AsNoTracking()
                .Where(sg => sg.Tenant.SubDomain == subDomain)
                .AverageAsync(sg => (double?)sg.Score / sg.TotalMarks * 100, cancellationToken) ?? 0;

            return new StudentStatisticsDto
            {
                Students = studentsCount,
                AverageGrade = averageGrade,
                AttendanceRate = 0,
                ActiveStudents = 0
            };
        }
        public async Task<bool> DeleteStudentAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

            if (enrollment is null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task CreateStudentAsync(Student student, CancellationToken cancellationToken)
        {
            const int maxRetries = 5;
            for (int i = 0; i < maxRetries; i++)
            {
                var exists = await _context.Students.AnyAsync(s => s.InviteCode == student.InviteCode, cancellationToken);
                if (!exists)
                {
                    await _context.Students.AddAsync(student, cancellationToken);
                    return;
                }
                student.RegenerateInviteCode();
            }
        }
        public async Task<StudentDto?> GetTenantStudentAsync(int studentId, string subDomain, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .AsNoTracking()
                .Where(s => s.Id == studentId && s.Enrollments.Any(e => e.Course.Tenant.SubDomain == subDomain))
                .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return student;

        }
        public async Task<List<AvailableSubjectDto>> GetAvailableSubjectsAsync(CancellationToken cancellationToken)
        {
            return await _context.AvailableSubjects
                .AsNoTracking()
                .Where(s => s.Active)
                .ProjectTo<AvailableSubjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task UpdateHasOnboardedAsync(string userId, CancellationToken cancellationToken)
        {
            await _context.Users
                .Where(s => s.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.HasOnboarded, true), cancellationToken);
        }
        public async Task<string> GetStudentEmailAsync(string userId, CancellationToken cancellationToken)
        {
            var result = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task<string> GetStuentNameByIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var studentName = await _context.Students
                .Where(s => s.Id == studentId)
                .Select(s => s.User.FirstName + " " + s.User.LastName)
                .FirstOrDefaultAsync(cancellationToken);
            return studentName!;
        }
        public async Task<Student?> GetStudentByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.InviteCode == inviteCode, cancellationToken);
        }
        public async Task<Student> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
            return student!;
        }
        public async Task<CurrentStudentDto> GetCurrentStudentAsync(string userId, int studentId, CancellationToken cancellationToken)
        {
            var result = await _context.Students
                .AsNoTracking()
                .Where(s => s.Id == studentId && s.UserId == userId)
                .ProjectTo<CurrentStudentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            var studentLevel = await _context.Levels
                .Where(l => l.RequiredXp > result!.Gamification.Xp)
                .OrderBy(l => l.RequiredXp)
                .Select(l => new { l.RequiredXp, l.LevelNumber })
                .FirstOrDefaultAsync(cancellationToken);

            result!.Gamification.Level = studentLevel!.LevelNumber;
            result!.Gamification.NextLevelXp = studentLevel.RequiredXp - result.Gamification.Xp;
            return result;
        }
        public Task UpdateStudentXPAsync(int studentId, int xpGained, CancellationToken cancellationToken)
        {
            return _context.Students
                .Where(s => s.Id == studentId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.XP, p => p.XP + xpGained), cancellationToken);
        }
    }
}