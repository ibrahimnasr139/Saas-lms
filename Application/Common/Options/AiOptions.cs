namespace Infrastructure.Common.Options
{
    public sealed class AiOptions
    {
        public string QuestionEndPoint { get; set; } = string.Empty;
        public string AiAssistantEndPoint { get; set; } = string.Empty;
        public string AskAiEndPoint { get; set; } = string.Empty;
        public string GenerateFlashCardsEndPoint { get; set; } = string.Empty;
        public string GenerateQuizEndPoint { get; set; } = string.Empty;
    }
}