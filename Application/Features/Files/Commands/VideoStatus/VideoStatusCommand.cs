namespace Application.Features.Files.Commands.VideoStatus
{
    public record VideoStatusCommand(string Id, string Status, long? Size) : IRequest<Unit> { }
}
