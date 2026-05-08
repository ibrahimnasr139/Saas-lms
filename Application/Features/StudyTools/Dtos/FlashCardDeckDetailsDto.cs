namespace Application.Features.StudyTools.Dtos
{
    public sealed class FlashCardDeckDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public StatsDto Stats { get; set; } = new();
        public List<CardDto> Cards { get; set; } = [];
    }
}