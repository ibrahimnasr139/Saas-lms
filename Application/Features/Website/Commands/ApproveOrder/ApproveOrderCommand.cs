using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.ApproveOrder
{
    public sealed record ApproveOrderCommand(int OrderId) : IRequest<OneOf<TenantOrderResponse, Error>>;
}
