using Application.Features.Attempts.Dtos;

namespace Infrastructure.Repositories
{
    public sealed class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;
        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task UpdateTeacherScore(int attemptId, List<QuestionDto> questions, CancellationToken cancellationToken)
        {
            foreach (var question in questions)
            {
                await _context.Answers
                    .Where(a => a.AttemptId == attemptId && a.QuizQuestionId == question.QuestionId && a.QuizQuestion.Marks >= question.TeacherScore)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.TeacherScore, question.TeacherScore)
                                              .SetProperty(a => a.Feedback, question.Feedback)
                                              .SetProperty(a => a.IsCorrect, question.TeacherScore >=
                                              _context.QuizQuestions.Where(q => q.Id == question.QuestionId).Select(q => q.Marks / 2).FirstOrDefault())
                                              , cancellationToken);
            }
        }
    }
}