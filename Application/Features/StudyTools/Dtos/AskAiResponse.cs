namespace Application.Features.StudyTools.Dtos
{
    public sealed class AskAiResponse
    {
        public string Question { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<string> Examples { get; set; } = [];
    }
}