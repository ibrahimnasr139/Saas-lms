using Domain.Abstractions;

namespace Domain.Entites
{
    public sealed class FlashCardDeck : IAuditable
    {
        public int Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Goal { get; set; }
        public string Source { get; set; } = string.Empty;
        public DateTime? LastReviewedAt { get; set; }
        public DateTime? NextReviewAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int SubjectId { get; set; }
        public StudentSubject StudentSubject { get; set; } = null!;
        public int ChapterId { get; set; }
        public StudentChapter StudentChapter { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public ICollection<FlashCard> FlashCards { get; set; } = [];
    }
}