namespace Application.Features.StudentLessons.Dtos
{
    public sealed class AiChatMessageDto
    {
        public string Message { get; init; } = string.Empty;
        public string Answer { get; init; } = string.Empty;
    }
}