using System.Text.Json;

namespace Domain.Entites
{
    public sealed class BlockType
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public JsonDocument Schema { get; set; } = JsonDocument.Parse("{}");
        public ICollection<PageBlock> PageBlocks { get; set; } = [];
    }
}