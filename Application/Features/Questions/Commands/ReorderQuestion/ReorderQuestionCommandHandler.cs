using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.ReorderQuestion
{
    internal sealed class ReorderQuestionCommandHandler : IRequestHandler<ReorderQuestionCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IQuestionRepository _questionRepository;


        public ReorderQuestionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository,
            IQuestionRepository questionRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _questionRepository = questionRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(ReorderQuestionCommand request, CancellationToken cancellationToken)
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

            var orderedQuestions = new Dictionary<int, int>();
            int order = 1;
            foreach (var questionId in request.QuestionIds)
                orderedQuestions.Add(questionId, order++);

            await _questionRepository.ReorderQuestions(request.ItemId, orderedQuestions, cancellationToken);
            return true;
        }
    }
}
