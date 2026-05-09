namespace Domain.Entites
{
    public sealed class FlashCard
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public byte Confidence { get; set; }
        public DateTime? LastReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int DeckId { get; set; }
        public FlashCardDeck FlashCardDeck { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public ICollection<FlashCardReview> FlashCardReviews { get; set; } = [];
    }
}