namespace Application.Features.Plan.DTOs
{
    public sealed class PlanDto
    {

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<PlanPricingDto> PlanPricing { get; set; } = [];
        public IEnumerable<PlanFeatureDto> PlanFeatures { get; set; } = [];
    }
}
