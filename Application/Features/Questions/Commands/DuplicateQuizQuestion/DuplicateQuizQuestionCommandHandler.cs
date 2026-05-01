using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.DuplicateQuizQuestion
{
    internal sealed class DuplicateQuizQuestionCommandHandler : IRequestHandler<DuplicateQuizQuestionCommand, OneOf<SuccessDto, Error>>
    {

        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DuplicateQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IQuizRepository quizRepository,
            IQuestionRepository questionRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(DuplicateQuizQuestionCommand request, CancellationToken cancellationToken)
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

            var question = await _questionRepository.GetQuestion(request.QuestionId, subdomain!, cancellationToken);
            Question duplicatedQuestion = _mapper.Map<Question>(question);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var id = await _questionRepository.CreateQuestion(duplicatedQuestion, cancellationToken);
                var duplicatedQuizQuestion = _mapper.Map<QuizQuestion>(quizQuestion);
                duplicatedQuizQuestion.QuestionId = id;
                await _questionRepository.AddQuestionsToQuiz(new List<QuizQuestion> { duplicatedQuizQuestion }, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new SuccessDto { Message = $"{nameof(Question)} {SuccessConstants.ItemDuplicated}" };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
