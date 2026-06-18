using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.DeletePaymentMethod
{
    public sealed record DeletePaymentMethodCommand(int paymentMethodId) : IRequest<OneOf<PaymentMethodResponseMessage, Error>>;
}
