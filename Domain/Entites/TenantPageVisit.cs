using Domain.Enums;

namespace Domain.Entites
{
    public sealed class TenantPageVisit
    {
        public int Id { get; set; }
        public string PageUrl { get; set; } = string.Empty;
        public bool Converted { get; set; }
        public int? DurationSecond { get; set; }
        public Guid VisitorId { get; set; }
        public DeviceType DeviceType { get; set; }
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }
}