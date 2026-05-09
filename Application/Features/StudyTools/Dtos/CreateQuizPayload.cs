using Domain.Enums;

namespace Application.Features.StudyTools.Dtos
{
    public sealed class CreateQuizPayload
    {
        public string Subject { get; set; } = string.Empty;
        public string? Chapter { get; set; }
        public int NumberOfQuestions { get; set; }
        public StudentQuizDifficulty Difficulty { get; set; }
    }
}