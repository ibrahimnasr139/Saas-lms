namespace Application.Features.Website.Dtos
{
    public sealed class TenantOrderStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int DeclinedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
