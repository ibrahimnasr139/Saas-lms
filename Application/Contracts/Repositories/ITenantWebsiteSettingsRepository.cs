using Application.Features.TenantWebsiteSettings.Commands.UpdateTenantWebsiteSettings;
using Application.Features.TenantWebsiteSettings.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantWebsiteSettingsRepository
    {
        Task<TenantWebsiteSettingsDto> GetSettingsAsync(string subDomain, CancellationToken cancellationToken);
        Task<bool> UpdateSettingsAsync(string subDomain, UpdateTenantWebsiteSettingsCommand update, CancellationToken cancellationToken);
    }
}
