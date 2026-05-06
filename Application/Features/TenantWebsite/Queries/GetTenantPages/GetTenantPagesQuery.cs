using Application.Features.TenantWebsite.Dtos;
namespace Application.Features.TenantWebsite.Queries.GetTenantPages
{
    public sealed record GetTenantPagesQuery : IRequest<List<TenantPagesDto>>;
}
