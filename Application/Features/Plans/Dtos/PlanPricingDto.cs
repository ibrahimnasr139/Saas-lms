namespace Application.Features.Plan.DTOs
{
    public sealed class PlanPricingDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Discount { get; set; }
    }
}