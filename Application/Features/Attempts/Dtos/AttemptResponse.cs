using Domain.Enums;

namespace Application.Features.Attempts.Dtos
{
    public sealed class AttemptResponse
    {
        public StudentDto Student { get; set; } = null!;
        public GradingStatus GradingStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public int? TimeSpent { get; set; }
        public double? Score { get; set; }
        public int TotalMarks { get; set; }
        public int QuestionCount { get; set; }
        public SummaryDto Summary { get; set; } = null!;
        public IEnumerable<QuestionAttempt> Questions { get; set; } = null!;
    }
}
