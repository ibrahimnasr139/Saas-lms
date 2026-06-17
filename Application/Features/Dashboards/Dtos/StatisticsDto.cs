namespace Application.Features.Dashboards.Dtos
{
    public sealed class StatisticsDto
    {
        public decimal Value { get; set; }
        public decimal Change { get; set; }
        public string Label { get; set; } = string.Empty;
        public StatisticStatus Status { get; set; }

    }
    public enum StatisticStatus
    {
        Up,
        Down,
    }
}