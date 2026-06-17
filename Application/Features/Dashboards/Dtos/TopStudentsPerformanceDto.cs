namespace Application.Features.Dashboards.Dtos
{
    public sealed class TopStudentsPerformanceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public int OverallScore { get; set; }
        public BreakdownDto Breakdown { get; set; } = new BreakdownDto();
    }
}