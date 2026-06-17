namespace Application.Features.Dashboards.Dtos
{
    public sealed class DashboardPerformanceDto
    {
        public List<ChartDataDto> ChartData { get; set; } = [];
        public int NewStudents { get; set; } 
        public decimal RevenueThisMonth { get; set; } 
        public int CompletedLessons { get; set; } 
    }
}