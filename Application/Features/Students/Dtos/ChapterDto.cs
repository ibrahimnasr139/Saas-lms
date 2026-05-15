namespace Application.Features.Students.Dtos
{
    public sealed class ChapterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCurrentChapter { get; set; }
        public int TotalLessons { get; set; }
    }
}