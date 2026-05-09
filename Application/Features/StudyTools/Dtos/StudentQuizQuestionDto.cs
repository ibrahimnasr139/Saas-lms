using Domain.Enums;

namespace Application.Features.StudyTools.Dtos
{
    public sealed class StudentQuizQuestionDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<StudentQuizQuestionOptionDto>? Options { get; set; } = new();
        public string Explanation { get; set; } = string.Empty;
        public StudentQuizQuestionType Type { get; set; }
    }
    public sealed class StudentQuizQuestionOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}