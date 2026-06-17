namespace Application.Features.Dashboards.Dtos
{
    public sealed class ChartDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Students { get; set; }
    }
}