namespace Application.Features.Questions.Dtos
{
    public sealed class CategoryStatisticsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}
