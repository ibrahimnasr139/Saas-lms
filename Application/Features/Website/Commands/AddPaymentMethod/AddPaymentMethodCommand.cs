using Application.Features.Website.Dtos;
using Domain.Enums;
using System.Text.Json;

namespace Application.Features.Website.Commands.AddPaymentMethod
{
    public sealed record AddPaymentMethodCommand(PaymentMethodType Type, Dictionary<string, JsonElement> Details)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
