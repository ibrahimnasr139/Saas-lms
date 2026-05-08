using Domain.Enums;

namespace Domain.Entites
{
    public sealed class FlashCardReview
    {
        public int Id { get; set; }
        public FlashCardDifficulty Difficulty { get; set; } 
        public int TimeSpentSeconds { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int FlashCardId { get; set; }
        public FlashCard FlashCard { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}