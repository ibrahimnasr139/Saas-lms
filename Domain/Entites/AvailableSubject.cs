namespace Domain.Entites
{
    public sealed class AvailableSubject
    {
        public int Id { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public ICollection<StudentChapter> StudentChapters { get; set; } = [];
        public ICollection<StudentSubject> StudentSubjects { get; set; } = [];
    }
}