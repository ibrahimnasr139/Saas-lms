namespace Domain.Entites
{
    public sealed class StudentAnswer
    {
        public int Id { get; set; }
        public string StudentAnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int StudentQuizQuestionId { get; set; }
        public StudentQuizQuestion StudentQuizQuestion { get; set; } = null!;
        public int StudentQuizAttemptId { get; set; }
        public StudentQuizAttempt StudentQuizAttempt { get; set; } = null!;
    }
}