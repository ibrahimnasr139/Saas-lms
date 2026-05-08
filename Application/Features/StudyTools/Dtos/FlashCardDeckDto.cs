namespace Application.Features.StudyTools.Dtos
{
    public sealed class FlashCardDeckDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int TotalCards { get; set; }
        public int KnownCards { get; set; }
        public int LearningCards { get; set; }
        public int NewCards { get; set; }
        public int Progress { get; set; }
        public string? Goal { get; set; }
    }
}