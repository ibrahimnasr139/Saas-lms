using Application.Features.Public.Dtos;
using Application.Features.Website.Commands.CreateTenantPage;
using Application.Features.Website.Commands.UpdateTenantPage;
using Application.Features.Website.Dtos;

namespace Application.Contracts.Repositories
{
    public interface ITenantPageRepository
    {
        Task CreateTenantPageAsync(CreateTenantPageCommand request, int tenantId, CancellationToken cancellationToken);
        Task CreateTenantPagesAsync(List<TenantPage> tenantPages , CancellationToken cancellationToken);
        Task<int> DeleteTenantPageAsync(int tenantId, int pageId, CancellationToken cancellationToken);
        Task DuplicateTenantPageAsync(TenantPage tenantPage, CancellationToken cancellationToken);
        Task<List<TenantPagesDto>> GetTenantPagesAsync(string subDomain, CancellationToken cancellationToken);
        Task<TenantPage?> GetTenantPageAsync(int tenantId, int pageId, CancellationToken cancellationToken);
        Task<TenantPageBlocksDto?> GetBlocksTypeAsync(CancellationToken cancellationToken);
        Task<bool> UrlExistsAsync(string subDomain, string url, CancellationToken cancellationToken);
        Task<bool> UpdateTenantPageAsync(int pageId, int tenantId, UpdateTenantPageCommand update, CancellationToken cancellationToken);
        Task<TenantPageDto?> GetTenantPageWithBlockTypeAsync(string subDomain, int pageId, CancellationToken cancellationToken);
        Task<List<TenantCourseDto>> GetTenantWebsiteCoursesAsync(string subDomain, List<int> courseIds, CancellationToken cancellationToken);
        Task<List<TenantNavigationLinkDto>> GetTenantNavigationLinksAsync(int tenantId, CancellationToken cancellationToken);
        Task<TenantPageDto?> GetPublishedTenantPagesAsync(string url, string subDomain, CancellationToken cancellationToken);
        Task<TenantPage> GetTenantPageByUrlAsync(string subDomain, string url, CancellationToken cancellationToken);
        Task CreateWebsiteSettingAsync(WebsiteSetting websiteSetting, CancellationToken cancellationToken);
        Task CreateEmailSettingsAsync(EmailSetting emailSetting, CancellationToken cancellationToken);
        Task CreateWebsiteAppearanceSettingAsync(WebsiteAppearanceSetting appearanceSetting, CancellationToken cancellationToken);
    }
}