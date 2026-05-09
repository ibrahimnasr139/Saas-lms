namespace Application.Features.StudyTools.Dtos
{
    public sealed class StudentAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}