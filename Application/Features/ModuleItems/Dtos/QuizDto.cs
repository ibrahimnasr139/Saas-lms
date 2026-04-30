namespace Application.Features.ModuleItems.Dtos
{
    public sealed class QuizDto
    {
        public int Duration { get; set; }
        public int PassingScore { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShuffleQuestions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<QuizQuestionDto> Questions { get; set; } = null!;
    }
}