namespace Application.Features.Website.Dtos
{
    public sealed class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public int Revenue { get; set; }
    }
}