using Application.Features.StudentQuizes.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentQuizes.Commands.StartQuiz
{
    internal sealed class StartQuizCommandHandler : IRequestHandler<StartQuizCommand, OneOf<StudentQuizResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartQuizCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
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
        public async Task<OneOf<StudentQuizResponse, Error>> Handle(StartQuizCommand request, CancellationToken cancellationToken)
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

            var isAttempted = await _quizRepository.IsStudentAttemptedAsync(session.StudentId, request.ItemId, cancellationToken);
            if (isAttempted)
                return AttemptErrors.AttemptAlreadyExists;

            var quiz = await _quizRepository.GetQuizAsync(request.ItemId, cancellationToken);
            var newAttempt = new QuizAttempt
            {
                ModuleItemId = request.ItemId,
                StudentId = session.StudentId,
                TotalMarks = quiz.TotalMarks,
                GradingStatus = GradingStatus.NotGraded,
                SubmissionStatus = SubmissionStatus.InProgress
            };
            await _quizRepository.CreateQuizAttemptAsync(newAttempt, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new StudentQuizResponse { Message = MessagesConstants.StartQuizSuccessfully };
        }
    }
}