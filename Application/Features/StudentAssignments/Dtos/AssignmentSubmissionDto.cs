using Domain.Enums;

namespace Application.Features.StudentAssignments.Dtos
{
    public sealed class AssignmentSubmissionDto
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public AssignmentStatus Status { get; set; }
        public int Score { get; set; }
        public string? Feedback { get; set; }
        public int GradedBy { get; set; }
        public DateTime GradedAt { get; set; }
        public DateTime SubmittedAt { get; set; }
        public SubmissionFileDto? SubmissionFiles { get; set; }
        public string? Link { get; set; }
        public string? Text { get; set; }
    }
}
