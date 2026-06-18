using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.UpdatePaymentMethodStatus
{
    public sealed record UpdatePaymentMethodStatusCommand(int PaymentMethodId, bool IsActive)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
