using Domain.Enums;

namespace Application.Features.Attempts.Dtos
{
    public sealed class AttemptDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string? ProfilePicture { get; set; }
        public SubmissionStatus SubmissionStatus { get; set; }
        public GradingStatus GradingStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public int? TimeSpent { get; set; }
        public double? Score { get; set; }
        public int TotalMarks { get; set; }
        public bool? IsPassed { get; set; }
    }
}
