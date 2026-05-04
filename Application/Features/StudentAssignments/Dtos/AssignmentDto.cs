using Domain.Enums;

namespace Application.Features.StudentAssignments.Dtos
{
    public sealed class AssignmentDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public SubmissionType SubmissionType { get; set; }
        public DateTime DueDate { get; set; }
        public int TotalMarks { get; set; }
        public List<Attachment> Attachments { get; set; } = [];
        public ModuleItemStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
