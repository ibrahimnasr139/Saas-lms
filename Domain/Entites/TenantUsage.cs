using Domain.Abstractions;

namespace Domain.Entites
{
    public sealed class TenantUsage : IAuditable
    {
        public int Id { get; set; }
        public long Used { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid PlanFeatureId { get; set; }
        public PlanFeature PlanFeature { get; set; } = null!;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;
    }
}
