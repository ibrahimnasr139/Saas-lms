using Application.Features.Public.Dtos;
using Domain.Enums;

namespace Application.Features.Website.Dtos
{
    public sealed class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<TenantPage, TenantPagesDto>();

            CreateMap<BlockType, BlockTypeDto>();

            CreateMap<PageBlock, TenantBlockTypeDto>()
                .ForMember(dest => dest.BlockType, opt => opt.MapFrom(src => src.BlockTypeId))
                .ForMember(dest => dest.Schema, opt => opt.MapFrom(src => src.BlockType.Schema.RootElement))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.BlockType.Description))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.BlockType.DisplayName));

            CreateMap<TenantMember, InstructorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));

            CreateMap<Course, TenantCourseDto>()
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.CourseStatus == CourseStatus.Published))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject.Label))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.Label))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Tenant.TenantMembers.FirstOrDefault()));

            CreateMap<TenantPage, TenantNavigationLinkDto>();

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.PricingType, opt => opt.MapFrom(src => src.PricingType.ToString()))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.BillingCycle.HasValue ? src.BillingCycle.Value.ToString() : null))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester));

            CreateMap<Order, TenantOrderDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.Timeline, opt => opt.MapFrom(src => src.OrderTimeLines));

            CreateMap<OrderTimeLine, TimelineDto>();

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

            CreateMap<PaymentMethod, PaymentMethodDto>();
        }
    }
}