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
        private readonly IStudentSubjectRepository _studentSubjectRepository;
        public CreateFlashCardDeckCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IOptions<AiOptions> options, IExternalService externalService, IFlashCardRepository flashCardRepository,
            IUnitOfWork unitOfWork, IStudentStreakRepository studentStreakRepository, IStudentSubjectRepository studentSubjectRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _flashCardRepository = flashCardRepository;
            _unitOfWork = unitOfWork;
            _options = options.Value;
            _studentStreakRepository = studentStreakRepository;
            _studentSubjectRepository = studentSubjectRepository;
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

            var subjectName = await _studentSubjectRepository.GetSubjectNameAsync(session.StudentId, request.SubjectId, cancellationToken);
            if (subjectName is null)
                return SubjectErrors.SubjectNotFound;

            string? chapterName = null;
            if (request.ChapterId.HasValue)
            {
                chapterName = await _studentSubjectRepository.GetChapterNameAsync(request.SubjectId, request.ChapterId.Value, cancellationToken);
                if (chapterName is null)
                    return SubjectErrors.ChapterNotFound;
            }

            var payload = new CreateFlashCardDeckRequest
            {
                Subject = subjectName,
                Chapter = chapterName,
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