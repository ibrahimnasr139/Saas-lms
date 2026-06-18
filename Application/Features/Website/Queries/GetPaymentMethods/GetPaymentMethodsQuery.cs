using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetPaymentMethods
{
    public sealed record GetPaymentMethodsQuery : IRequest<OneOf<List<PaymentMethodDto>, Error>>;
}
