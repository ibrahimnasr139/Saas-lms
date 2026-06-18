using Application.Features.Website.Dtos;
using Domain.Enums;

namespace Application.Features.Website.Commands.UpdateTenantPage
{
    public sealed record UpdateTenantPageCommand(int pageId, string Title, string Url, TenantPageStatus Status,
        string? MetaTitle, string? MetaDescription, List<PageBlocksDto> PageBlocks) : IRequest<OneOf<TenantPageResponse, Error>>;
}
