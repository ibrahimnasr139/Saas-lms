using Domain.Enums;

namespace Application.Features.Questions.Dtos
{
    public sealed class QuestionResponse
    {
        public int Id { get; set; }
        public string? CorrectAnswer { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Question { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<QuestionOption>? Options { get; set; }
    }
}
