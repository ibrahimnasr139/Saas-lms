namespace Application.Features.StudyTools.Dtos
{
    public sealed class CreateFlashCardDeckRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string? Chapter { get; set; }
        public int NumberOfCards { get; set; }
        public string? Goal { get; set; }
        public string Topic { get; set; } = string.Empty;
    }
}