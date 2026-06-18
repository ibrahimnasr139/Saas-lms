using Domain.Enums;

namespace Application.Features.Tenants.Dtos
{
    public sealed class SubscriptionDto
    {
        public int Id { get; set; }
        public SubscriptionPlanDto Plan { get; set; } = new SubscriptionPlanDto();
        public SubscriptionStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}