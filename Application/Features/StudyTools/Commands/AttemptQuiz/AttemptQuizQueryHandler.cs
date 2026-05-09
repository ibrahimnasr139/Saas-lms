using Application.Features.StudyTools.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudyTools.Commands.AttemptQuiz
{
    internal sealed class AttemptQuizQueryHandler : IRequestHandler<AttemptQuizQuery, OneOf<AttemptQuizDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentQuizRepository _studentQuizRepository;
        public AttemptQuizQueryHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            IStudentQuizRepository studentQuizRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _studentQuizRepository = studentQuizRepository;
        }
        public async Task<OneOf<AttemptQuizDto, Error>> Handle(AttemptQuizQuery request, CancellationToken cancellationToken)
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

            var studentQuizExists = await _studentQuizRepository.StudentQuizIsExistAsync(request.QuizId, session.StudentId, cancellationToken);
            if (!studentQuizExists)
                return StudentQuizErrors.NotFound;

            var answers = request.Answers.Select(a => (a.QuestionId, a.SelectedOptionIndex)).ToList();
            var selectedOptionsText = await _studentQuizRepository.GetSelectedOptionsTextAsync(request.QuizId, session.StudentId, answers, cancellationToken);
            
            var newAttempt = new StudentQuizAttempt
            {
                Score = (byte)request.Score,
                TimeSpent = request.TimeSpent,
                StudentId = session.StudentId,
                StudentQuizId = request.QuizId,
                StudentAnswers = request.Answers.Select((a, i) => new StudentAnswer
                {
                    StudentAnswerText = selectedOptionsText[i],
                    StudentQuizQuestionId = a.QuestionId,
                    IsCorrect = a.IsCorrect,
                }).ToList(),
            };
            await _studentQuizRepository.CreateStudentQuizAttemptAsync(newAttempt, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new AttemptQuizDto
            {
                AttemptId = newAttempt.Id,
                Score = request.Score,
                Passed = request.Passed
            };
        }
    }
}