using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.DeclineOrder
{
    public sealed record DeclineOrderCommand(int OrderId, string? Reason) : IRequest<OneOf<TenantOrderResponse, Error>>;
}
