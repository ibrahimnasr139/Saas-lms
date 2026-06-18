using Application.Features.Website.Dtos;

namespace Application.Features.Website.Commands.DuplicateTenantPage
{
    public sealed record DuplicateTenantPageCommand(int PageId) : IRequest<OneOf<TenantPageResponse, Error>>;
}
