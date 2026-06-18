using System.Text.Json;

namespace Application.Features.Website.Dtos
{
    public sealed class BlockTypeDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JsonDocument Schema { get; set; } = JsonDocument.Parse("{}");
        public string Icon { get; set; } = string.Empty;
    }
}
