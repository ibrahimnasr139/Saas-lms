namespace Domain.Entites
{
    public sealed class StudentSubject
    {
        public int Id { get; set; }
        public int Confidence { get; set; }
        public int Progress { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CurrentChaper { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int AvailableSubjectId { get; set; }
        public AvailableSubject AvailableSubject { get; set; } = null!;
        public ICollection<StudentChapter> StudentChapters { get; set; } = [];
        public ICollection<FlashCardDeck> FlashCardDecks { get; set; } = [];
    }
}