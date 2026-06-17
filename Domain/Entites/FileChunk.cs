namespace Domain.Entites
{
    public sealed class FileChunk
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public int Tokens { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public Dictionary<string, string>? Metadata { get; set; }
        public string FileId { get; set; } = string.Empty;
        public File File { get; set; } = null!;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }
}