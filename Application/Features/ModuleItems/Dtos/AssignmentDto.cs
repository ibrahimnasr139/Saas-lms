using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class AssignmentDto
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public string Instructions { get; set; } = null!;
        public int TotalMarks { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public List<Attachment> Attachments { get; set; } = [];
    }
}
