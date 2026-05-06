using Domain.Enums;

namespace Application.Features.Quizzes.Dtos
{
    public sealed class AiResponse
    {
        public string Question { get; set; } = string.Empty;
        public int Marks { get; set; }
        public QuestionType Type { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool RequiresManualGrading { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}
