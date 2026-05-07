namespace Application.Features.StudentLessons.Dtos
{
    public sealed class TranscriptDto
    {
        public int Id { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}