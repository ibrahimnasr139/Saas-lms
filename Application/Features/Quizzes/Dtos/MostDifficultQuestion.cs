namespace Application.Features.Quizzes.Dtos
{
    public sealed class MostDifficultQuestion
    {
        public string Question { get; set; } = string.Empty;
        public int InCorrectAnswers { get; set; }
        public int TotalAnswers { get; set; }
        public double PercentageIncorrect => TotalAnswers == 0 ? 0 : (double)InCorrectAnswers / TotalAnswers * 100;
    }
}
