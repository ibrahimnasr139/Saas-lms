using Application.Features.StudyTools.Dtos;

namespace Application.Features.StudyTools.Queries.GetFlashCardDecks
{
    public sealed record GetFlashCardDecksQuery : IRequest<OneOf<List<FlashCardDeckDto>, Error>>;
}