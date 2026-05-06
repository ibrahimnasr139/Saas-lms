namespace Application.Features.Questions.Dtos
{
    public sealed class QuestionStatisticsDto
    {
        public int TotalQuestions { get; set; }
        public int UsedQuestions { get; set; }
        public int NewThisWeek { get; set; }
        public IEnumerable<CategoryStatisticsDto> Categories { get; set; } = [];
        public IEnumerable<QuestionTypeDto> QuestionsByType { get; set; } = [];
    }
}
