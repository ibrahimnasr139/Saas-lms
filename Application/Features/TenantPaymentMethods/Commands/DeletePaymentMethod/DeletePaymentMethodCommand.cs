using Application.Features.TenantPaymentMethods.Dtos;

namespace Application.Features.TenantPaymentMethods.Commands.DeletePaymentMethod
{
    public sealed record DeletePaymentMethodCommand(int paymentMethodId) : IRequest<OneOf<PaymentMethodResponseMessage, Error>>;
}
