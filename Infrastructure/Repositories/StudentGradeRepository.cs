namespace Infrastructure.Repositories
{
    internal sealed class StudentGradeRepository : IStudentGradeRepository
    {
        private readonly AppDbContext _context;
        public StudentGradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateStudentGradeAsync(StudentGrade studentGrade, CancellationToken cancellationToken)
        {
            await _context.AddAsync(studentGrade, cancellationToken);
        }
        public async Task<StudentGrade?> GetStudentGradeAsync(int studentId, int itemId, CancellationToken cancellationToken)
        {
            return await _context.StudentGrades
                .FirstOrDefaultAsync(sg => sg.StudentId == studentId && sg.TypeId == itemId, cancellationToken);
        }
    }
}