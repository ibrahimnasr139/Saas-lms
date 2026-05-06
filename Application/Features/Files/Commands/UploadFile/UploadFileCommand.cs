using Application.Features.Files.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Files.Commands.UploadFile
{
    public sealed record UploadFileCommand(IFormFile File, string? Folder, string? Name, bool EnableEmbedding = false)
        : IRequest<OneOf<UploadFileDto, Error>>;
}
