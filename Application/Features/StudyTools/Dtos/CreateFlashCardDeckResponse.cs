namespace Application.Features.StudyTools.Dtos
{
    public sealed class CreateFlashCardDeckResponse
    {
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
    }
}