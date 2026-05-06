using Domain.Enums;

namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class StudentQuestionDto
    {
        public int Id { get; set; }
        public int Order { get; set; } 
        public QuestionType Type { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Marks { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<QuestionOptionDto>? Options { get; set; }
        public QuestionAnswerDto? Answer { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }
}