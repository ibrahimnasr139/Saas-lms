using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.UpdateTenantWebsiteSettings
{
    public sealed record UpdateTenantWebsiteSettingsCommand(GeneralDto? General, EmailDto? Email, NotificationsDto? Notifications,
        AppearanceDto? Appearance) : IRequest<OneOf<TenantWebsiteSettingsResponse, Error>>;
}
