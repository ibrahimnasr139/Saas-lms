using Application.Features.Website.Dtos;
using Domain.Enums;

namespace Application.Features.Website.Commands.CreateTenantPage
{
    public sealed record CreateTenantPageCommand(string Title, string Url, TenantPageStatus Status, string? MetaTitle,
        string? MetaDescription, List<PageBlocksDto> PageBlocks) : IRequest<OneOf<TenantPageResponse, Error>>;
}