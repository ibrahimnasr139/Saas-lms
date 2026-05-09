namespace Domain.Entites
{
    public sealed class StudentChapter
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int ChapterNumber { get; set; }
        public string SkillTag { get; set; } = string.Empty;
        public Dictionary<string, string>? Metadata { get; set; }
        public int SubjectId { get; set; }
        public StudentSubject StudentSubject { get; set; } = null!;
        public ICollection<FlashCardDeck> FlashCardDecks { get; set; } = [];
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = [];
    }
}