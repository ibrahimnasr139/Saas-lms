using Application.Features.Website.Dtos;
namespace Application.Features.Website.Queries.GetTenantPages
{
    public sealed record GetTenantPagesQuery : IRequest<List<TenantPagesDto>>;
}
