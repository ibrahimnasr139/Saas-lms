using Application.Features.Website.Dtos;
using System.Text.Json;

namespace Application.Features.Website.Commands.UpdatePaymentMethod
{
    public sealed record UpdatePaymentMethodCommand(int PaymentMethodId, Dictionary<string, JsonElement> Details)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
