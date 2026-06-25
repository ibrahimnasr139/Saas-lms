namespace Application.Features.StudyTools.Dtos
{
    public sealed class AskAiRequest
    {
        public string Question { get; set; } = string.Empty;
        public string? PreviousAnswer { get; set; }
        public string? Grade { get; set; }
    }
}