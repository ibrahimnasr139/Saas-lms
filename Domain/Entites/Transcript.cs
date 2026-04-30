namespace Domain.Entites
{
    public sealed class Transcript
    {
        public int Id { get; set; }
        public string Language { get; set; } = string.Empty;
        public string FullText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FileId { get; set; } = string.Empty;
        public File File { get; set; } = null!;
    }
}