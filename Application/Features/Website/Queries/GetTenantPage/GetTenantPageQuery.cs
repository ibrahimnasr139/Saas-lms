using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetTenantPage
{
    public sealed record GetTenantPageQuery(int PageId) : IRequest<OneOf<TenantPageDto, Error>>;
}
