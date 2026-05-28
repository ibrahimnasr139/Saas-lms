namespace Application.Features.Tenants.Dtos
{
    public sealed class VideoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Duration { get; set; }
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
