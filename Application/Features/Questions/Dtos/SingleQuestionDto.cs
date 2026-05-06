using Domain.Enums;

namespace Application.Features.Questions.Dtos
{
    public sealed class SingleQuestionDto
    {
        public int Id { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Question { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public CategoryDto Category { get; set; } = null!;
        public int Reuse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
