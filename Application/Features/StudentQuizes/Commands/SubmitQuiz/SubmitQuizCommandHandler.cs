using Application.Features.StudentQuizes.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentQuizes.Commands.SubmitQuiz
{
    internal sealed class SubmitQuizCommandHandler : IRequestHandler<SubmitQuizCommand, OneOf<StudentQuizResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SubmitQuizCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentSubscriptionRepository studentSubscriptionRepository, IEnrollmentRepository enrollmentRepository,
            IModuleItemRepository moduleItemRepository, IQuizRepository quizRepository, IUnitOfWork unitOfWork)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _moduleItemRepository = moduleItemRepository;
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentQuizResponse, Error>> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
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

            var isEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!isEnrolled)
                return StudentCourseErrors.StudentNotEnrolledInCourse;

            var subscriptionIsActive = await _studentSubscriptionRepository.StudentSubscriptionIsActiveAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!subscriptionIsActive)
                return StudentSubscriptionErrors.StudentSubscribedExpired;

            var moduleItemIsExist = await _moduleItemRepository.ModuleItemIsExistAsync(request.ItemId, request.CourseId, cancellationToken);
            if (!moduleItemIsExist)
                return ModuleItemErrors.ModuleItemNotFound;

            var attempt = await _quizRepository.GetStudentAttemptedAsync(session.StudentId, request.ItemId, cancellationToken);
            if (attempt is null)
                return AttemptErrors.AttemptNotFound;

            var quizQuestionIds = request.Answers.Select(a => a.QuestionId).ToList();
            var answerValues = request.Answers.Select(a => a.Value).ToList();
            var questionIdsWithAnswers = await _quizRepository.GetQuestionIdsAsync(request.ItemId, quizQuestionIds, answerValues, attempt.Id, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var score = await _quizRepository.GradeQuizAttemptAsync(questionIdsWithAnswers, cancellationToken);

                attempt.Score = score;
                attempt.GradingStatus = GradingStatus.NeedsGrading;
                attempt.SubmissionStatus = SubmissionStatus.Submitted;
                attempt.TimeSpent = request.TimeSpent;
                await _quizRepository.UpdateQuizAttempt(attempt);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            return new StudentQuizResponse { Message = MessagesConstants.SubmitQuizSuccessfully };
        }
    }
}