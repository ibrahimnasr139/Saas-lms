using Domain.Enums;

namespace Application.Features.ModuleItems.Dtos
{
    public sealed class QuizQuestionDto
    {
        public int Id { get; set; }
        public string? CorrectAnswer { get; set; }
        public string Question { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int Category { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public int Marks { get; set; }
        public int Order { get; set; }
    }
}
