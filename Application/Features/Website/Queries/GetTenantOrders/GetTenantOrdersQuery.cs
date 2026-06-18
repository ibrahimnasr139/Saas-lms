using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetTenantOrders
{
    public sealed record GetTenantOrdersQuery : IRequest<List<TenantOrderDto>>;
}
