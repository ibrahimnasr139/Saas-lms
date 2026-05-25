using Application.Features.Files.Dtos;

namespace Application.Features.Files.Commands.CreateUpload
{
    public sealed record CreateUploadCommand(string Title, long Size, int ThumbnailTime, double Duration)
        : IRequest<OneOf<CreateUploadDto, Error>>;
}