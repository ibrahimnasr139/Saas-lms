namespace Application.Features.Website.Dtos
{
    public sealed class NotificationsDto
    {
        public EmailSettingsDto Email { get; set; } = new EmailSettingsDto();
    }
}