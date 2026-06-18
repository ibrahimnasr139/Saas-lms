using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetSettings
{
    public sealed record GetTenantWebsiteSettingsQuery : IRequest<OneOf<TenantWebsiteSettingsDto, Error>>;
}
