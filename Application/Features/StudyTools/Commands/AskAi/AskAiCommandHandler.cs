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
        private readonly IUnitOfWork _unitOfWork;
        private readonly AiOptions _options;
        private readonly IStudentStreakRepository _studentStreakRepository;
        private readonly IStudentRepository _studentRepository;

        public AskAiCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IOptions<AiOptions> options,
            IExternalService externalService, IStudentStreakRepository studentStreakRepository, IStudentRepository studentRepository,
            IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _unitOfWork = unitOfWork;
            _options = options.Value;
            _studentStreakRepository = studentStreakRepository;
            _studentRepository = studentRepository;
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

            var grade = await _studentRepository.GetStudentGradeAsync(session.StudentId, cancellationToken);
            var payload = new AskAiRequest
            {
                Question = request.Question,
                PreviousAnswer = request.PreviousAnswer,
                Grade = grade
            };

            var endpoint = _options.AskAiEndPoint;
            var result = await _externalService.CallExternalServiceAsync<AskAiRequest, AskAiResponse>(endpoint, payload, cancellationToken);
            if (result is null)
                throw new Exception();

            await _studentStreakRepository.UpdateStudentStreakAsync(session.StudentId, cancellationToken, true);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new AskAiDto
            {
                Question = result.Question,
                Examples = result.Examples,
                Explanation = result.Explanation
            };
        }
    }
}