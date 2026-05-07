namespace Application.Features.StudentLessons.Dtos
{
    public sealed class AiAssistantRequest
    {
        public string Message { get; set; } = string.Empty;
        public string FileId { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Lesson { get; set; } = string.Empty;
    }
}