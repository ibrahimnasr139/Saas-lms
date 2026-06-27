namespace Application.Features.Tenants.Dtos
{
    public sealed class ExpiredSubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
    }
}