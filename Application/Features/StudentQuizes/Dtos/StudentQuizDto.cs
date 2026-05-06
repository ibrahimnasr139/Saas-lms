namespace Application.Features.StudentQuizes.Dtos
{
    public sealed class StudentQuizDto
    {
        public QuizDto Quiz { get; set; } = new();
        public StudentAttemptDto Attempt { get; set; } = new();
        public List<StudentQuestionDto> Questions { get; set; } = new();
    }
}