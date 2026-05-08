using Application.Features.StudyTools.Dtos;

namespace Infrastructure.Repositories
{
    public sealed class FlashCardRepository : IFlashCardRepository
    {
        private readonly AppDbContext _context;

        public FlashCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateFlashCardDeckAsync(FlashCardDeck flashCardDeck, CancellationToken cancellationToken)
        {
            await _context.FlashCardDecks.AddAsync(flashCardDeck, cancellationToken);
        }
        public async Task CreateFlashCardReviewAsync(FlashCardReview flashCardReview, CancellationToken cancellationToken)
        {
            await _context.FlashCardReviews.AddAsync(flashCardReview, cancellationToken);
        }
        public async Task<FlashCard?> GetFlashCardAsync(int flashcardId, int deckId, int studentId, CancellationToken cancellationToken)
        {
            return await _context.FlashCards
                .Include(fc => fc.FlashCardDeck)
                .FirstOrDefaultAsync(fc => fc.Id == flashcardId && fc.DeckId == deckId && fc.StudentId == studentId, cancellationToken);
        }
        public async Task<List<FlashCardDeckDto>> GetFlashCardDecksAsync(int studentId, CancellationToken cancellationToken)
        {
            return await _context.FlashCardDecks
                .Where(deck => deck.StudentId == studentId)
                .Select(deck => new FlashCardDeckDto
                {
                    Id = deck.Id,
                    Title = deck.Title,
                    Subject = deck.StudentSubject.AvailableSubject.DisplayName,
                    TotalCards = deck.FlashCards.Count,
                    LearningCards = deck.FlashCards.Count(fc => fc.Confidence < 85 && fc.Confidence > 40),
                    KnownCards = deck.FlashCards.Count(fc => fc.Confidence >= 85),
                    NewCards = deck.FlashCards.Count(fc => fc.Confidence < 40),
                    Progress = deck.FlashCards.Count == 0
                        ? 0
                        : (int)((double)deck.FlashCards.Count(fc => fc.Confidence >= 85) / deck.FlashCards.Count * 100),
                    Goal = deck.Goal
                }).ToListAsync(cancellationToken);
        }
        public async Task<FlashCardDeckDetailsDto?> GetFlashCardDeckDetailsAsync(int deckId, int studentId, CancellationToken cancellationToken)
        {
            return await _context.FlashCardDecks
                .Where(deck => deck.Id == deckId && deck.StudentId == studentId)
                .Select(deck => new FlashCardDeckDetailsDto
                {
                    Id = deck.Id,
                    Title = deck.Title,
                    Subject = deck.StudentSubject.AvailableSubject.DisplayName,
                    Stats = new StatsDto
                    {
                        TotalCards = deck.FlashCards.Count,
                        LearningCards = deck.FlashCards.Count(fc => fc.Confidence < 85 && fc.Confidence > 40),
                        KnownCards = deck.FlashCards.Count(fc => fc.Confidence >= 85),
                        NewCards = deck.FlashCards.Count(fc => fc.Confidence < 40),
                        Progress = deck.FlashCards.Count == 0
                            ? 0
                            : (int)((double)deck.FlashCards.Count(fc => fc.Confidence >= 85) / deck.FlashCards.Count * 100)
                    },
                    Cards = deck.FlashCards.Select(fc => new CardDto
                    {
                        Id = fc.Id,
                        Front = fc.Front,
                        Back = fc.Back,
                        Confidence = fc.Confidence,
                        LastReviewedAt = fc.LastReviewedAt
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<(string, string)> GetSubjectNameAndChapterNameAsync(int studentId, int subjectId, int chapterId, CancellationToken cancellationToken)
        {
            var result = await _context.StudentChapters
                .Where(sc => sc.Id == chapterId && sc.SubjectId == subjectId && sc.StudentSubject.StudentId == studentId)
                .Select(sc => new
                {
                    SubjectName = sc.StudentSubject.AvailableSubject.DisplayName,
                    ChapterName = sc.Title
                }).FirstOrDefaultAsync(cancellationToken);

            return (result!.SubjectName, result.ChapterName);
        }
        public async Task<bool> FlashCardDeckIsExistAsync(int deckId, CancellationToken cancellationToken)
        {
            return await _context.FlashCardDecks.AnyAsync(fd => fd.Id == deckId, cancellationToken);
        }
    }
}