using Application.Features.TenantOrders.Dtos;
using Domain.Enums;

namespace Application.Features.TenantOrders.Commands.BulkOrderAction
{
    public sealed record BulkOrderActionCommand(List<int> OrderIds, OrderStatus Action, string? Reason)
        : IRequest<OneOf<TenantOrderResponse, Error>>;
}
