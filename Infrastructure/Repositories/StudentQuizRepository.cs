namespace Infrastructure.Repositories
{
    public sealed class StudentQuizRepository : IStudentQuizRepository
    {
        private readonly AppDbContext _context;

        public StudentQuizRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateStudentQuizAsync(StudentQuiz studentQuiz, CancellationToken cancellationToken)
        {
            await _context.StudentQuizzes.AddAsync(studentQuiz, cancellationToken);
        }
    }
}