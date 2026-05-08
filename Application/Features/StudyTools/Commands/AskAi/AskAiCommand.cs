using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Commands.AskAi
{
    public sealed record AskAiCommand(string Question, string? PreviousAnswer) : IRequest<OneOf<AskAiDto, Error>>;
}