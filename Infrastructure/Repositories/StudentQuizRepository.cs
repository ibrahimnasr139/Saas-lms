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
        public async Task CreateStudentQuizAttemptAsync(StudentQuizAttempt studentQuizAttempt, CancellationToken cancellationToken)
        {
            await _context.StudentQuizAttempts.AddAsync(studentQuizAttempt, cancellationToken);
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
        public async Task<bool> StudentQuizIsExistAsync(int quizId, int studentId, CancellationToken cancellationToken)
        {
            return await _context.StudentQuizzes.AnyAsync(q => q.Id == quizId && q.StudentId == studentId, cancellationToken);
        }
        public async Task<List<string>> GetSelectedOptionsTextAsync(int quizId, int studentId, List<(int QuestionId, int OptionIndex)> answers, CancellationToken cancellationToken)
        {
            var questions = await _context.StudentQuizQuestions
                .Where(qq => qq.StudentQuizId == quizId && qq.StudentQuiz.StudentId == studentId)
                .Select(qq => new { qq.Id, qq.Options })
                .ToListAsync(cancellationToken);

            return answers.Select(a =>
            {
                var question = questions.First(q => q.Id == a.QuestionId);
                return question.Options[a.OptionIndex].Text;
            }).ToList();
        }
    }
}