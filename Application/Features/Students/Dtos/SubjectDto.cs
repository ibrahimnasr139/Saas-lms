namespace Application.Features.Students.Dtos
{
    public sealed class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Progress { get; set; }
        public int Confidence { get; set; }
        public int TotalMissions { get; set; } = 0;
        public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();
    }
}