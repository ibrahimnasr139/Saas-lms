using Domain.Enums;

namespace Application.Features.TenantWebsiteSettings.Dtos
{
    public sealed class AppearanceDto
    {
        public string? Logo { get; set; }
        public string? Favicon { get; set; }
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public DirectionType Direction { get; set; }
    }
}
