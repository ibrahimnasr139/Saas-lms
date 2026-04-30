namespace Domain.Entites
{
    public sealed class PlanFeature
    {
        public Guid Id { get; set; }
        public int? LimitValue { get; set; }
        public string LimitUnit { get; set; } = string.Empty;
        public bool? IsEnabled { get; set; }
        public string? Note { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
        public Guid FeatureId { get; set; }
        public Feature Feature { get; set; } = null!;
        public ICollection<TenantUsage> TenantUsages { get; set; } = [];
    }
}
