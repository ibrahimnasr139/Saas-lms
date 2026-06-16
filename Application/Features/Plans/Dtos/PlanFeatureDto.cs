namespace Application.Features.Plan.DTOs
{
    public sealed class PlanFeatureDto
    {
        public string Id { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureDescription { get; set; } = string.Empty;
        public string FeatureKey { get; set; } = string.Empty;
        public int? LimitValue { get; set; }
        public string LimitUnit { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool? IsEnabled { get; set; }
    }
}