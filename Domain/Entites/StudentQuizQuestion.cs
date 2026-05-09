using Domain.Enums;

namespace Domain.Entites
{
    public sealed class StudentQuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public StudentQuizQuestionOption Option { get; set; }
        public StudentQuizQuestionType Type { get; set; }
        public int StudentQuizId { get; set; }
        public StudentQuiz StudentQuiz { get; set; } = null!;
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    }
}