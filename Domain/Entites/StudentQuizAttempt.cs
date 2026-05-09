namespace Domain.Entites
{
    public sealed class StudentQuizAttempt
    {
        public int Id { get; set; }
        public byte Score { get; set; }
        public int TimeSpent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int StudentQuizId { get; set; }
        public StudentQuiz StudentQuiz { get; set; } = null!;
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    }
}