using Domain.Enums;

namespace Application.Features.Files.Commands.VideoStatus
{
    public sealed record VideoStatusCommand(string Id, FileStatus Status, long? Size) : IRequest<Unit>;
}