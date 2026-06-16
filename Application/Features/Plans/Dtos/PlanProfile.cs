namespace Application.Features.Plan.DTOs
{
    public sealed class PlanProfile : Profile
    {
        public PlanProfile()
        {
            CreateMap<Domain.Entites.Plan, PlanDto>()
             .ForMember(dest => dest.PlanPricing, opt => opt.MapFrom(src => src.PlanPricings))
             .ForMember(dest => dest.PlanFeatures, opt => opt.MapFrom(src => src.PlanFeatures));

            CreateMap<PlanPricing, PlanPricingDto>()
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.BillingCycle.ToString()))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountPercent));

            CreateMap<PlanFeature, PlanFeatureDto>()
                .ForMember(dest => dest.FeatureName, opt => opt.MapFrom(src => src.Feature.Name))
                .ForMember(dest => dest.FeatureDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FeatureKey, opt => opt.MapFrom(src => src.Feature.Key));
        }
    }
}
