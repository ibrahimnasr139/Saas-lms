namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class QuizDeadlineReminderDto
    {
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string QuizTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EndDate { get; set; }
    }
}