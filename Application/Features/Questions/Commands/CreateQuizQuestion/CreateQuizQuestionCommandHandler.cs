using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.CreateQuizQuestion
{
    internal sealed class CreateQuizQuestionCommandHandler : IRequestHandler<CreateQuizQuestionCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantRepository _tenantRepository;
        public CreateQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository, IQuestionRepository questionRepository,
            IUnitOfWork unitOfWork, IMapper mapper, ITenantRepository tenantRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantRepository = tenantRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateQuizQuestionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var quiz = await _moduleItemRepository.GetQuizAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (quiz is null)
                return ModuleItemErrors.ModuleItemNotFound;

            if (request.Category.HasValue)
            {
                var isFound = await _questionRepository.IsFoundCategory(request.Category.Value, cancellationToken);
                if (!isFound)
                    return QuestionErrors.CategoryNotFound;
            }
            var question = _mapper.Map<Question>(request);
            question.TenantId = await _tenantRepository.GetTenantIdAsync(subdomain!, cancellationToken);
            var quizQuestion = _mapper.Map<QuizQuestion>(request);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var id = await _questionRepository.CreateQuestion(question, cancellationToken);
                quizQuestion.QuizId = quiz.ModuleItemId;
                quizQuestion.QuestionId = id;
                await _questionRepository.AddQuestionsToQuiz(new List<QuizQuestion> { quizQuestion }, cancellationToken);
                quiz.TotalMarks += request.Marks; 
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
