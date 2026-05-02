namespace Application.Features.TenantWebsiteSettings.Dtos
{
    public sealed class TenantWebsiteSettingsProfile : Profile
    {
        public TenantWebsiteSettingsProfile()
        {
            // Read
            CreateMap<Tenant, TenantWebsiteSettingsDto>()
                .ForMember(dest => dest.General, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.WebsiteAppearnceSetting ?? new WebsiteAppearanceSetting()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailSetting ?? new EmailSetting()))
                .ForMember(dest => dest.Notifications, opt => opt.MapFrom(src => src.NotificationSetting ?? new NotificationSetting()));

            CreateMap<Tenant, GeneralDto>()
                .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.PlatformName))
                .ForMember(dest => dest.Tagline, opt => opt.MapFrom(src => src.WebsiteSetting != null ? src.WebsiteSetting.TagLine : null))
                .ForMember(dest => dest.IsMaintenanceMode, opt => opt.MapFrom(src => src.WebsiteSetting != null && src.WebsiteSetting.IsMaintenanceMode))
                .ForMember(dest => dest.HomepageId, opt => opt.MapFrom(src => src.TenantPages.Select(p => p.Id).FirstOrDefault()));

            CreateMap<WebsiteAppearanceSetting, AppearanceDto>()
                .ForMember(dest => dest.Favicon, opt => opt.MapFrom(src => src.FavIcon))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Tenant.Logo));

            CreateMap<EmailSetting, EmailDto>();

            CreateMap<NotificationSetting, NotificationsDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src));

            CreateMap<NotificationSetting, EmailSettingsDto>();


            // Updated
            CreateMap<GeneralDto, WebsiteSetting>()
                .ForMember(dest => dest.TagLine, opt => opt.MapFrom(src => src.Tagline))
                .ForMember(dest => dest.IsMaintenanceMode, opt => opt.MapFrom(src => src.IsMaintenanceMode))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<EmailDto, EmailSetting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AppearanceDto, WebsiteAppearanceSetting>()
                .ForMember(dest => dest.FavIcon, opt => opt.MapFrom(src => src.Favicon))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<NotificationsDto, NotificationSetting>()
                .ForMember(dest => dest.OrderApproved, opt => opt.MapFrom(src => src.Email.OrderApproved))
                .ForMember(dest => dest.OrderSubmitted, opt => opt.MapFrom(src => src.Email.OrderSubmitted))
                .ForMember(dest => dest.OrderRejected, opt => opt.MapFrom(src => src.Email.OrderRejected))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}