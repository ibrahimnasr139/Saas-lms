namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class QuizAnswerDto
    {
        public int QuestionId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}