using Application.Features.TenantPaymentMethods.Dtos;

namespace Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethodStatus
{
    public sealed record UpdatePaymentMethodStatusCommand(int PaymentMethodId, bool IsActive)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
