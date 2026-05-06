using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Commands.DeleteContentLibraryResource
{
    public record DeleteContentLibraryResourceCommand(string FileId) : IRequest<OneOf<DeleteContentLibraryResourceResponse, Error>> { }
}
