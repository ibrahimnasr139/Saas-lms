namespace Application.Features.StudyTools.Dtos
{
    public sealed class CardDto
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public byte Confidence { get; set; }
        public DateTime? LastReviewedAt { get; set; }
    }
}