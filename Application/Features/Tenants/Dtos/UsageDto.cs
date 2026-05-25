namespace Application.Features.Tenants.Dtos
{
    public sealed class UsageDto
    {
        public string FeatureKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Used { get; set; }
        public int? Limit { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool? Enabled { get; set; }
    }
}
