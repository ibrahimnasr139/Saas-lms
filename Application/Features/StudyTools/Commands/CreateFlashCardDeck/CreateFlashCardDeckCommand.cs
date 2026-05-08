using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Commands.CreateFlashCardDeck
{
    public sealed record CreateFlashCardDeckCommand(int SubjectId, int ChapterId, int NumberOfCards, string Title, string? Goal, string Topic)
        : IRequest<OneOf<CreateFlashCardDeckDto, Error>>;
}