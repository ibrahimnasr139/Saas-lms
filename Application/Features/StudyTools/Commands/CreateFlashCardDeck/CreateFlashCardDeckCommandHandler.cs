using Application.Contracts.Externals;
using Application.Features.StudyTools.Dtos;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.StudyTools.Commands.CreateFlashCardDeck
{
    internal sealed class CreateFlashCardDeckCommandHandler : IRequestHandler<CreateFlashCardDeckCommand, OneOf<CreateFlashCardDeckDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExternalService _externalService;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly AiOptions _options;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentStreakRepository _studentStreakRepository;
        public CreateFlashCardDeckCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IOptions<AiOptions> options, IExternalService externalService, IFlashCardRepository flashCardRepository,
            IUnitOfWork unitOfWork, IStudentStreakRepository studentStreakRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _flashCardRepository = flashCardRepository;
            _unitOfWork = unitOfWork;
            _options = options.Value;
            _studentStreakRepository = studentStreakRepository;
        }
        public async Task<OneOf<CreateFlashCardDeckDto, Error>> Handle(CreateFlashCardDeckCommand request, CancellationToken cancellationToken)
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

            (string SubjectName, string ChapterName) = await _flashCardRepository.GetSubjectNameAndChapterNameAsync(session.StudentId, request.SubjectId, request.ChapterId, cancellationToken);

            var payload = new CreateFlashCardDeckRequest
            {
                Subject = SubjectName,
                Chapter = ChapterName,
                Goal = request.Goal,
                Topic = request.Topic,
                NumberOfCards = request.NumberOfCards
            };
            var endpoint = _options.GenerateFlashCardsEndPoint;
            var result = await _externalService.CallExternalServiceAsync<CreateFlashCardDeckRequest, List<CreateFlashCardDeckResponse>>(endpoint, payload, cancellationToken);

            if (result is null)
                throw new Exception();

            var newDeck = new FlashCardDeck
            {
                Title = request.Title,
                Goal = request.Goal,
                Topic = request.Topic,
                Source = "AI",
                StudentId = session.StudentId,
                ChapterId = request.ChapterId,
                SubjectId = request.SubjectId,
                FlashCards = result.Select(fc => new FlashCard
                {
                    Front = fc.Front,
                    Back = fc.Back,
                    StudentId = session.StudentId
                }).ToList()
            };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _flashCardRepository.CreateFlashCardDeckAsync(newDeck, cancellationToken);
                await _studentStreakRepository.UpdateStudentStreakAsync(session.StudentId, cancellationToken, true);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            return new CreateFlashCardDeckDto { Id = newDeck.Id };
        }
    }
}