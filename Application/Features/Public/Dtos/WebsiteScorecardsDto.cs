namespace Application.Features.Public.Dtos
{
    public sealed class WebsiteScorecardsDto
    {
        public int Visitors { get; set; }
        public int PageViews { get; set; }
        public double ConversionRate { get; set; }
        public string AverageSessionDuration { get; set; } = string.Empty;
    }
}