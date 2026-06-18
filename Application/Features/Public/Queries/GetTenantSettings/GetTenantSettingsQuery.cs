using Application.Features.Website.Dtos;

namespace Application.Features.Public.Queries.GetTenantSettings
{
    public sealed record GetTenantSettingsQuery : IRequest<TenantWebsiteSettingsDto>;
}
