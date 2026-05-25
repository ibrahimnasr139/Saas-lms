using Domain.Enums;

namespace Application.Features.Files.Commands.VideoStatus
{
    public record VideoStatusCommand(string Id, FileStatus Status, long? Size) : IRequest<Unit>;
}