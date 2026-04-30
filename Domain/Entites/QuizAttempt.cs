using Domain.Enums;

namespace Domain.Entites
{
    public sealed class QuizAttempt
    {
        public int Id { get; set; }
        public int ModuleItemId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int TotalMarks { get; set; }
        public double? Score { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public int? TimeSpent { get; set; }
        public GradingStatus GradingStatus { get; set; }
        public SubmissionStatus SubmissionStatus { get; set; }
        public ICollection<Answer> Answers { get; set; } = [];
    }
}
