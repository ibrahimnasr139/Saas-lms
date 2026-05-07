namespace Application.Features.StudentLessons.Dtos
{
    public sealed class StudentLessonTranscriptDto
    {
        public int TotalSegments { get; set; }
        public List<TranscriptDto> Transcript { get; set; } = new();
    }
}