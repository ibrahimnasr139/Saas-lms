using Application.Features.StudyTools.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Queries.GetFlashCardDeckDetails
{
    internal sealed class GetFlashCardDeckDetailsQueryHandler : IRequestHandler<GetFlashCardDeckDetailsQuery, OneOf<FlashCardDeckDetailsDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlashCardRepository _flashCardRepository;
        public GetFlashCardDeckDetailsQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IFlashCardRepository flashCardRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _flashCardRepository = flashCardRepository;
        }
        public async Task<OneOf<FlashCardDeckDetailsDto, Error>> Handle(GetFlashCardDeckDetailsQuery request, CancellationToken cancellationToken)
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

            var flashCardDeckIsExist = await _flashCardRepository.FlashCardDeckIsExistAsync(request.DeckId, cancellationToken);
            if (!flashCardDeckIsExist)
                return FlashCardErrors.NotFound;

            return await _flashCardRepository.GetFlashCardDeckDetailsAsync(request.DeckId, session.StudentId, cancellationToken)
                ?? new FlashCardDeckDetailsDto();
        }
    }
}