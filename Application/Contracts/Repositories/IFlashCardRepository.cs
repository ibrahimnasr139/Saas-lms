using Application.Features.StudyTools.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IFlashCardRepository
    {
        Task CreateFlashCardDeckAsync(FlashCardDeck flashCardDeck, CancellationToken cancellationToken);
        Task<List<FlashCardDeckDto>> GetFlashCardDecksAsync(int studentId, CancellationToken cancellationToken);
        Task<(string, string)> GetSubjectNameAndChapterNameAsync(int studentId, int subjectId, int chapterId, CancellationToken cancellationToken);
        Task<bool> FlashCardDeckIsExistAsync(int deckId, CancellationToken cancellationToken);
        Task<FlashCardDeckDetailsDto?> GetFlashCardDeckDetailsAsync(int deckId, int studentId, CancellationToken cancellationToken);
    }
}