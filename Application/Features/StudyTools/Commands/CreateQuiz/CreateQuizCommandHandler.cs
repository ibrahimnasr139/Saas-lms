using Application.Contracts.Externals;
using Application.Features.StudyTools.Dtos;
using Domain.Enums;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.StudyTools.Commands.CreateQuiz
{
    internal sealed class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, OneOf<CreateQuizDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExternalService _externalService;
        private readonly AiOptions _options;
        private readonly IStudentSubjectRepository _studentSubjectRepository;
        private readonly IStudentQuizRepository _studentQuizRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentStreakRepository _studentStreakRepository;

        public CreateQuizCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IOptions<AiOptions> options,
            IExternalService externalService, IStudentSubjectRepository studentSubjectRepository, IStudentQuizRepository studentQuizRepository,
            IUnitOfWork unitOfWork, IStudentStreakRepository studentStreakRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _options = options.Value;
            _studentSubjectRepository = studentSubjectRepository;
            _studentQuizRepository = studentQuizRepository;
            _unitOfWork = unitOfWork;
            _studentStreakRepository = studentStreakRepository;
        }
        public async Task<OneOf<CreateQuizDto, Error>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
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

            (string subject, string? chapter) = await _studentSubjectRepository.GetSubjectAndChapterNamesAsync(request.SubjectId, request.ChapterId, session.StudentId, cancellationToken);

            var payload = new CreateQuizPayload
            {
                Subject = subject,
                Topic = request.Topic,
                Chapter = chapter,
                NumberOfQuestions = request.NumberOfQuestions,
                Difficulty = request.Difficulty.ToString().ToLower()
            };

            var endpoint = _options.GenerateQuizEndPoint;
            var result = await _externalService.CallExternalServiceAsync<CreateQuizPayload, List<CreateQuizResponse>>(endpoint, payload, cancellationToken);

            if (result is null)
                throw new Exception();

            var newStudentQuiz = new StudentQuiz
            {
                TimeLimit = request.NumberOfQuestions * 120,
                Difficulty = request.Difficulty,
                StudentId = session.StudentId,
                SubjectId = request.SubjectId,
                ChapterId = request.ChapterId,
                StudentQuizQuestions = result.Select(r => new StudentQuizQuestion
                {
                    Question = r.Question,
                    Explanation = r.Explanation,
                    Type = Enum.TryParse<StudentQuizQuestionType>(r.Type, true, out var type) ? type : StudentQuizQuestionType.Mcq,
                    Options = r.Options.Select(o => new StudentQuizQuestionOption
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };
            await _studentQuizRepository.CreateStudentQuizAsync(newStudentQuiz, cancellationToken);
            await _studentStreakRepository.UpdateStudentStreakAsync(session.StudentId, cancellationToken, true);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new CreateQuizDto { Id = newStudentQuiz.Id };
        }
    }
}