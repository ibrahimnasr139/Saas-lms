namespace Application.Features.Website.Dtos
{
    public sealed class TenantPaymentMethodsProfile : Profile
    {
        public TenantPaymentMethodsProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodDto>();
        }
    }
}
