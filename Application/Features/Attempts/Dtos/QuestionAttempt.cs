using Domain.Enums;

namespace Application.Features.Attempts.Dtos
{
    public sealed class QuestionAttempt
    {
        public int QuestionId { get; set; }
        public string Question { get; set; } = string.Empty;
        public int Order { get; set; }
        public QuestionType Type { get; set; }
        public int Marks { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public AnswerDto? Answer { get; set; }
    }
}
