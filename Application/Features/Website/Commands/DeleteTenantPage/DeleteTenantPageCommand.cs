using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.DeleteTenantPage
{
    public sealed record DeleteTenantPageCommand(int PageId) : IRequest<OneOf<TenantPageResponse, Error>>;
}
