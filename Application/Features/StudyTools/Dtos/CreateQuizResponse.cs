using Domain.Enums;

namespace Application.Features.StudyTools.Dtos
{
    public sealed class CreateQuizResponse
    {
        public string Question { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<StudentQuizQuestionOption> Options { get; set; } = [];
        public StudentQuizQuestionType Type { get; set; }
    }
}