using System.Text.Json;

namespace Application.Features.TenantWebsite.Dtos
{
    public sealed class TenantBlockTypeDto
    {
        public int Id { get; set; }
        public string BlockType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool Visible { get; set; }
        public JsonElement Schema { get; set; }
        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();
    }
}
