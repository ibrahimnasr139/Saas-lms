using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Queries.GetContentLibraryResources
{
    public sealed record GetContentLibraryResourcesQuery(string? Q, string Type) : IRequest<ContentLibraryResourceDto>;
}