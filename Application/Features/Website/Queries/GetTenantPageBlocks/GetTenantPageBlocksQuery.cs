using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetTenantPageBlocks
{
    public sealed record GetTenantPageBlocksQuery : IRequest<TenantPageBlocksDto>;
}
