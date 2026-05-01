using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.ImportQuizQuestion
{
    internal sealed class ImportQuizQuestionCommandHandler : IRequestHandler<ImportQuizQuestionCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ImportQuizQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository,
            IQuestionRepository questionRepository, IUnitOfWork unitOfWork)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(ImportQuizQuestionCommand request, CancellationToken cancellationToken)
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

            var quizQuestions = new List<QuizQuestion>();
            foreach (var id in request.QuestionIds)
            {
                quizQuestions.Add(new QuizQuestion
                {
                    QuizId = request.ItemId,
                    QuestionId = id,
                });
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _questionRepository.AddQuestionsToQuiz(quizQuestions, cancellationToken);
                await _questionRepository.IncreaseReuse(request.QuestionIds, cancellationToken);
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
