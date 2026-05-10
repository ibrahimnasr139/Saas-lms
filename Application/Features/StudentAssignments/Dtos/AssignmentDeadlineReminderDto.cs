namespace Application.Features.StudentAssignments.Dtos
{
    public sealed class AssignmentDeadlineReminderDto
    {
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string AssignmentTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime DueDate { get; set; }
    }
}