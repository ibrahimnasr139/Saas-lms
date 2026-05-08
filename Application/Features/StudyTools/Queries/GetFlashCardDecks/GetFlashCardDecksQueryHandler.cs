using Application.Features.StudyTools.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Queries.GetFlashCardDecks
{
    internal sealed class GetFlashCardDecksQueryHandler : IRequestHandler<GetFlashCardDecksQuery, OneOf<List<FlashCardDeckDto>, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlashCardRepository _flashCardRepository;

        public GetFlashCardDecksQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IFlashCardRepository flashCardRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _flashCardRepository = flashCardRepository;
        }
        public async Task<OneOf<List<FlashCardDeckDto>, Error>> Handle(GetFlashCardDecksQuery request, CancellationToken cancellationToken)
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

            return await _flashCardRepository.GetFlashCardDecksAsync(session.StudentId, cancellationToken);
        }
    }
}