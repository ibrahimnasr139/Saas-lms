namespace Application.Features.Website.Dtos
{
    public sealed class TenantOrdersProfile : Profile
    {
        public TenantOrdersProfile()
        {
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
        }
    }
}