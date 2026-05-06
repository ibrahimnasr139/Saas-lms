namespace Domain.Entites
{
    public sealed class Answer
    {
        public int QuizQuestionId { get; set; }
        public QuizQuestion QuizQuestion { get; set; } = null!;
        public int AttemptId { get; set; }
        public QuizAttempt Attempt { get; set; } = null!;
        public string? StudentAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public bool? Selected { get; set; }
        public double? TeacherScore { get; set; }
        public double? AutoScore { get; set; }
        public string? Feedback { get; set; }
    }
}
