using Application.Features.Attempts.Dtos;
using Domain.Enums;

namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class StudentAttemptDto
    {
        public int Id { get; set; }
        public GradingStatus GradingStatus { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? TimeSpent { get; set; }
        public double? Score { get; set; }
        public int MaxScore { get; set; }
        public bool IsPublished { get; set; }
        public SummaryDto Summary { get; set; } = new();
    }
}