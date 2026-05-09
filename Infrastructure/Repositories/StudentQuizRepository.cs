using Application.Features.StudyTools.Dtos;

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
        public async Task<StudentQuizDto?> GetStudentQuizAsync(int quizId, int studentId, CancellationToken cancellationToken)
        {
            var quiz = await _context.StudentQuizzes
                .Where(q => q.Id == quizId && q.StudentId == studentId)
                .Select(q => new
                {
                    q.Id,
                    Subject = q.Subject.AvailableSubject.DisplayName,
                    Chapter = q.Chapter != null ? q.Chapter.Title : null,
                    q.Difficulty,
                    q.TimeLimit,
                    Questions = q.StudentQuizQuestions.Select(qq => new
                    {
                        qq.Id,
                        qq.Question,
                        qq.Options,
                        qq.Explanation
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (quiz is null)
                return null;

            return new StudentQuizDto
            {
                Id = quiz.Id,
                Subject = quiz.Subject,
                Chapter = quiz.Chapter,
                Difficulty = quiz.Difficulty,
                TimeLimit = quiz.TimeLimit,
                Questions = quiz.Questions.Select(qq => new StudentQuizQuestionDto
                {
                    Id = qq.Id,
                    Explanation = qq.Explanation,
                    Question = qq.Question,
                    Options = qq.Options.Select(o => new StudentQuizQuestionOptionDto
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };
        }
    }
}