using Application.Features.Tenants.Dtos;
using Domain.Enums;

namespace Application.Features.Tenants.Queries.GetContentLibraryResources
{
    public sealed record GetContentLibraryResourcesQuery(string? Q, FileType Type) : IRequest<ContentLibraryResourceDto>;
}