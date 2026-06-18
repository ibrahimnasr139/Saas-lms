namespace Application.Features.Website.Dtos
{
    public sealed class GeneralDto
    {
        public string PlatformName { get; set; } = string.Empty;
        public string? Tagline { get; set; }
        public int HomepageId { get; set; }
        public bool IsMaintenanceMode { get; set; }
    }
}