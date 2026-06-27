namespace Application.Features.Tenants.Dtos
{
    public sealed class ExpiringSubscriptionDto
    {
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public DateTime EndsAt { get; set; }
        public string SubDomain { get; set; } = string.Empty;
    }
}