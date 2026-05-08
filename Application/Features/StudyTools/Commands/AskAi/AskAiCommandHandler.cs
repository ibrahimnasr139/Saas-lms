using Application.Contracts.Externals;
using Application.Features.StudyTools.Dtos;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.StudyTools.Commands.AskAi
{
    internal sealed class AskAiCommandHandler : IRequestHandler<AskAiCommand, OneOf<AskAiDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExternalService _externalService;
        private readonly AiOptions _options;
        public AskAiCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IOptions<AiOptions> options,
            IExternalService externalService)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _options = options.Value;
        }
        public async Task<OneOf<AskAiDto, Error>> Handle(AskAiCommand request, CancellationToken cancellationToken)
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

            var payload = new AskAiRequest
            {
                Question = request.Question,
                PreviousAnswer = request.PreviousAnswer
            };

            var endpoint = _options.AskAiEndPoint;
            var result = await _externalService
                .CallExternalServiceAsync<AskAiRequest, AskAiResponse>(endpoint, payload, cancellationToken);
            if (result is null)
                throw new Exception();

            return new AskAiDto
            {
                Question = result.Question,
                Examples = result.Examples,
                Explanation = result.Explanation
            };
        }
    }
}