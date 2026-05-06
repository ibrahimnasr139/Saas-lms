using Application.Features.TenantPaymentMethods.Dtos;

namespace Application.Features.TenantPaymentMethods.Queries.GetPaymentMethods
{
    public sealed record GetPaymentMethodsQuery : IRequest<OneOf<List<PaymentMethodDto>, Error>>;
}
