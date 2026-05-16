using Application.Features.StudyTools.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Commands.ReviewFlashCard
{
    internal sealed class ReviewFlashCardCommandHandler : IRequestHandler<ReviewFlashCardCommand, OneOf<ReviewFlashCardDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentStreakRepository _studentStreakRepository;

        public ReviewFlashCardCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IFlashCardRepository flashCardRepository, IUnitOfWork unitOfWork, IStudentRepository studentRepository,
            IStudentStreakRepository studentStreakRepository)
        {
            _flashCardRepository = flashCardRepository;
            _unitOfWork = unitOfWork;
            _studentRepository = studentRepository;
            _studentStreakRepository = studentStreakRepository;
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

            var baseDelta = request.Difficulty switch
            {
                FlashCardDifficulty.Again => -10,
                FlashCardDifficulty.Hard => 5,
                FlashCardDifficulty.Good => 10,
                FlashCardDifficulty.Easy => 15,
                _ => 0
            };

            var timeMultiplier = request.ReviewTimeSeconds switch
            {
                <= 2 => 1.3,
                <= 5 => 1.1,
                <= 10 => 1.0,
                <= 20 => 0.8,
                _ => 0.6
            };

            var delta = baseDelta * timeMultiplier;
            var oldConfidence = flashCard.Confidence;
            var newConfidence = (byte)Math.Clamp(Math.Round(oldConfidence + delta), 0, 100);
            var xpGained = newConfidence - oldConfidence;
            if (xpGained > 0)
                await _studentRepository.UpdateStudentXPAsync(session.StudentId, xpGained, cancellationToken);

            flashCard.Confidence = newConfidence;
            flashCard.LastReviewedAt = DateTime.UtcNow;
            flashCard.FlashCardDeck.LastReviewedAt = DateTime.UtcNow;
            flashCard.FlashCardDeck.NextReviewAt = DateTime.UtcNow.AddDays(3);

            await _flashCardRepository.CreateFlashCardReviewAsync(newFlashCardReview, cancellationToken);
            await _studentStreakRepository.UpdateStudentStreakAsync(session.StudentId, cancellationToken, true);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new ReviewFlashCardDto { Message = MessagesConstants.FlashCardReviewedSuccessfully };
        }
    }
}