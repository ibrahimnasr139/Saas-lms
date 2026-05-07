using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.UpdateQuizQuestion
{
    internal sealed class UpdateQuizQuestionCommandHandler : IRequestHandler<UpdateQuizQuestionCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            IQuizRepository quizRepository, IQuestionRepository questionRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(UpdateQuizQuestionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var quizQuestion = await _quizRepository.GetQuizQuestion(request.ItemId, request.QuestionId, subdomain!, cancellationToken);
            if (quizQuestion is null)
                return QuestionErrors.QuestionNotFound;

            var isFound = await _questionRepository.IsFoundCategory(request.Category, cancellationToken);
            if (!isFound)
                return QuestionErrors.CategoryNotFound;

            var quiz = await _quizRepository.GetQuizAsync(request.ItemId, cancellationToken);
            var marks = request.Marks - quizQuestion.Marks;
            quiz.TotalMarks += marks;

            quizQuestion.Question.CorrectAnswer = request.CorrectAnswer;
            quizQuestion.Question.Difficulty = request.Difficulty;
            quizQuestion.Question.QuestionCategoryId = request.Category;
            quizQuestion.Question.QuestionTitle = request.Question;
            quizQuestion.Question.Type = request.Type;
            quizQuestion.Question.Options = request.Options;
            quizQuestion.Order = request.Order;
            quizQuestion.RequiresManualGrading = request.RequiresManualGrading;
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
