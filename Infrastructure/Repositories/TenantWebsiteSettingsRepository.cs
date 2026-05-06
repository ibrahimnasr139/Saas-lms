using Application.Features.TenantWebsiteSettings.Commands.UpdateTenantWebsiteSettings;
using Application.Features.TenantWebsiteSettings.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class TenantWebsiteSettingsRepository : ITenantWebsiteSettingsRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TenantWebsiteSettingsRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<TenantWebsiteSettingsDto> GetSettingsAsync(string subDomain, CancellationToken cancellationToken)
        {
            var result = await _context.Tenants
                .AsNoTracking()
                .Where(t => t.SubDomain == subDomain)
                .ProjectTo<TenantWebsiteSettingsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public async Task<bool> UpdateSettingsAsync(int tenantId, UpdateTenantWebsiteSettingsCommand update, CancellationToken cancellationToken)
        {
            var query = _context.Tenants
                .Where(t => t.Id == tenantId);

            if (update.General is not null)
                query = query.Include(t => t.WebsiteSetting);

            if (update.Email is not null)
                query = query.Include(t => t.EmailSetting);

            if (update.Appearance is not null)
                query = query.Include(t => t.WebsiteAppearnceSetting);

            if (update.Notifications is not null)
                query = query.Include(t => t.NotificationSetting);


            var tenant = await query.FirstOrDefaultAsync(cancellationToken);
            if (tenant is null) return false;

            if (update.General is not null)
            {
                tenant.PlatformName = update.General.PlatformName ?? tenant.PlatformName;
                _mapper.Map(update.General, tenant.WebsiteSetting);
            }

            if (update.Email is not null)
                _mapper.Map(update.Email, tenant.EmailSetting);

            if (update.Appearance is not null)
            {
                _mapper.Map(update.Appearance, tenant.WebsiteAppearnceSetting);
                if (!string.IsNullOrEmpty(update.Appearance.Logo))
                    tenant.Logo = update.Appearance.Logo;
            }

            if (update.Notifications is not null)
                _mapper.Map(update.Notifications, tenant.NotificationSetting);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
