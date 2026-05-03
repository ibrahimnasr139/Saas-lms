namespace Application.Features.TenantOrders.Dtos
{
    public sealed class TimelineDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Actor { get; set; } = string.Empty;
    }
}
