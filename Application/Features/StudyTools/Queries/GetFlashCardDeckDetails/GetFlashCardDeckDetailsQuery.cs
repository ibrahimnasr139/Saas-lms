using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Queries.GetFlashCardDeckDetails
{
    public sealed record GetFlashCardDeckDetailsQuery(int DeckId) : IRequest<OneOf<FlashCardDeckDetailsDto, Error>>;
}