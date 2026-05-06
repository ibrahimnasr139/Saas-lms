using Application.Features.TenantPaymentMethods.Dtos;
using Domain.Enums;
using System.Text.Json;

namespace Application.Features.TenantPaymentMethods.Commands.AddPaymentMethod
{
    public sealed record AddPaymentMethodCommand(PaymentMethodType Type, Dictionary<string, JsonElement> Details)
        : IRequest<OneOf<PaymentMethodResponse, Error>>;
}
