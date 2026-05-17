using Application.Features.StudyTools.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Repositories
{
    public interface IFlashCardRepository
    {
        Task CreateFlashCardDeckAsync(FlashCardDeck flashCardDeck, CancellationToken cancellationToken);
        Task CreateFlashCardReviewAsync(FlashCardReview flashCardReview, CancellationToken cancellationToken);
        Task<FlashCard?> GetFlashCardAsync(int flashcardId, int deckId, int studentId, CancellationToken cancellationToken);
        Task<List<FlashCardDeckDto>> GetFlashCardDecksAsync(int studentId, CancellationToken cancellationToken);
        Task<FlashCardDeckDetailsDto?> GetFlashCardDeckDetailsAsync(int deckId, int studentId, CancellationToken cancellationToken);
        Task<bool> FlashCardDeckIsExistAsync(int deckId, CancellationToken cancellationToken);
    }
}