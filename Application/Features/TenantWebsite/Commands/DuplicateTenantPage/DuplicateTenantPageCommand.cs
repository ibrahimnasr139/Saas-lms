using Application.Features.TenantWebsite.Dtos;

namespace Application.Features.TenantWebsite.Commands.DuplicateTenantPage
{
    public sealed record DuplicateTenantPageCommand(int PageId) : IRequest<OneOf<TenantPageResponse, Error>>;
}
