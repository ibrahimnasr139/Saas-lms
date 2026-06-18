using Application.Features.Website.Dtos;
using Domain.Enums;

namespace Application.Features.Website.Commands.BulkOrderAction
{
    public sealed record BulkOrderActionCommand(List<int> OrderIds, OrderStatus Action, string? Reason)
        : IRequest<OneOf<TenantOrderResponse, Error>>;
}
