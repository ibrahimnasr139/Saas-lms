namespace Application.Features.Dashboards.Dtos
{
    public sealed class DashboardStatisticsDto
    {
        public StatisticsDto TotalStudents { get; set; } = new StatisticsDto();
        public StatisticsDto TotalRevenue { get; set; } = new StatisticsDto();
        public StatisticsDto NewSubscriptions { get; set; } = new StatisticsDto();
        public StatisticsDto CompletionRate { get; set; } = new StatisticsDto();
    }
}