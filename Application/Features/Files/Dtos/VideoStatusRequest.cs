using Domain.Enums;

namespace Application.Features.Files.Dtos
{
    public sealed record VideoStatusRequest(FileStatus Status, long? Size);
}