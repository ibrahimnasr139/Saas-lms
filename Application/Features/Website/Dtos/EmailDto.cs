namespace Application.Features.Website.Dtos
{
    public sealed class EmailDto
    {
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string ReplyToEmail { get; set; } = string.Empty;
    }
}
