namespace Domain.Entites
{
    public sealed class Metadata
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public bool IsCourseBook { get; set; }
        public string FileId { get; set; } = string.Empty;
        public File File { get; set; } = null!;
    }
}