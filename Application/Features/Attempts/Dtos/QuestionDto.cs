namespace Application.Features.Attempts.Dtos
{
    public sealed class QuestionDto
    {
        public int QuestionId { get; set; }
        public double TeacherScore { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
