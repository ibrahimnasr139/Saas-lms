using Domain.Enums;

namespace Application.Features.StudyTools.Dtos
{
    public sealed class StudentQuizDto
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Chapter { get; set; }
        public List<StudentQuizQuestionDto> Questions { get; set; } = new();
        public StudentQuizDifficulty Difficulty { get; set; }
        public int TimeLimit { get; set; }
    }
}