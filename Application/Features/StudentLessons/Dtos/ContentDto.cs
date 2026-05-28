namespace Application.Features.StudentLessons.Dtos
{
    public sealed class ContentDto
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public double Duration { get; set; }
    }
}