using Application.Features.TenantPaymentMethods.Dtos;
using System.Text.Json;

namespace Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethod
{
    public sealed record UpdatePaymentMethodCommand(int PaymentMethodId, Dictionary<string, JsonElement> Details)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
