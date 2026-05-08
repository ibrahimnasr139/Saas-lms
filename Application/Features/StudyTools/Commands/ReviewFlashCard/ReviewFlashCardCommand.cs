using Application.Features.StudyTools.Dtos;
using Domain.Enums;

namespace Application.Features.StudyTools.Commands.ReviewFlashCard
{
    public sealed record ReviewFlashCardCommand(int DeckId, int FlashCardId, int ReviewTimeSeconds, FlashCardDifficulty Difficulty)
        : IRequest<OneOf<ReviewFlashCardDto, Error>>;
}