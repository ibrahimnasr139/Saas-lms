namespace Application.Features.Lessons.Dtos
{
    public sealed class LessonContentDto
    {
        public string Type { get; set; } = string.Empty;
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Duration { get; set; }
        public List<ResourceDto> Resources { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}