namespace Application.Features.Attempts.Dtos
{
    public sealed class AnswerDto
    {
        public string? Value { get; set; }
        public bool IsCorrect { get; set; }
        public double? TeacherScore { get; set; }
        public double? AutoScore { get; set; }
        public string? Feedback { get; set; }
    }
}
