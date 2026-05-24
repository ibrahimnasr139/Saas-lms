using Application.Features.Files.Dtos;

namespace Application.Features.Files.Commands.CreateUpload
{
    public sealed record CreateUploadCommand(string Title, int Size, int ThumbnailTime, int Duration) 
        : IRequest<OneOf<CreateUploadDto, Error>>;
}
