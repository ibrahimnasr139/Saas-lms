using Application.Features.StudyTools.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Commands.ReviewFlashCard
{
    internal sealed class ReviewFlashCardCommandHandler : IRequestHandler<ReviewFlashCardCommand, OneOf<ReviewFlashCardDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewFlashCardCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IFlashCardRepository flashCardRepository, IUnitOfWork unitOfWork)
        {
            _flashCardRepository = flashCardRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<ReviewFlashCardDto, Error>> Handle(ReviewFlashCardCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            var flashCard = await _flashCardRepository.GetFlashCardAsync(request.FlashCardId, request.DeckId, session.StudentId, cancellationToken);
            if (flashCard is null)
                return FlashCardErrors.NotFound;

            var newFlashCardReview = new FlashCardReview
            {
                Difficulty = request.Difficulty,
                TimeSpentSeconds = request.ReviewTimeSeconds,
                ReviewedAt = DateTime.UtcNow,
                FlashCardId = request.FlashCardId,
                StudentId = session.StudentId,
            };

            flashCard.LastReviewedAt = DateTime.UtcNow;
            flashCard.FlashCardDeck.LastReviewedAt = DateTime.UtcNow;
            flashCard.FlashCardDeck.NextReviewAt = DateTime.UtcNow.AddDays(3);

            await _flashCardRepository.CreateFlashCardReviewAsync(newFlashCardReview, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new ReviewFlashCardDto { Message = MessagesConstants.FlashCardReviewedSuccessfully };
        }
    }
}