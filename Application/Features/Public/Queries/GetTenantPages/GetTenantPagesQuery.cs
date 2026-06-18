using Application.Features.Website.Dtos;

namespace Application.Features.Public.Queries.GetTenantPages
{
    public sealed record GetTenantPagesQuery(string Url) : IRequest<OneOf<TenantPageDto, Error>>;
}
