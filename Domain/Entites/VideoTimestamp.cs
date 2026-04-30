namespace Domain.Entites
{
    public sealed class VideoTimestamp
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SegmentIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string FileId { get; set; } = string.Empty;
        public File File { get; set; } = null!;
    }
}