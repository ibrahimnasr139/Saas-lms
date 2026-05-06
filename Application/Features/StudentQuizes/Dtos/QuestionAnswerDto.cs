namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class QuestionAnswerDto
    {
        public string? Value { get; set; }
        public bool IsCorrect { get; set; }
        public double? Score { get; set; }
        public string? Feedback { get; set; }
    }
}