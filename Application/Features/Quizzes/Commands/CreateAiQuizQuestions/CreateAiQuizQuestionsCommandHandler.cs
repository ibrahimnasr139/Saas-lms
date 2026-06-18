using Application.Contracts.Externals;
using Application.Features.Quizzes.Dtos;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.Quizzes.Commands.CreateAiQuizQuestions
{
    internal sealed class CreateAiQuizQuestionsCommandHandler : IRequestHandler<CreateAiQuizQuestionsCommand, OneOf<bool, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IExternalService _externalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPlanRepository _planRepository;
        private readonly AiOptions _options;
        public CreateAiQuizQuestionsCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IExternalService externalService,
            IQuizRepository quizRepository, IQuestionRepository questionRepository, ITenantRepository tenantRepository, IUnitOfWork unitOfWork,
            IMapper mapper, IOptions<AiOptions> options, IPlanRepository planRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _planRepository = planRepository;
            _options = options.Value;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateAiQuizQuestionsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_QUIZZES, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var featureUsage = await _planRepository.GetFeatureUsageInfoAsync(tenantId, FeatureConstants.AI_CREDITS, cancellationToken);
            if (featureUsage is null)
                return TenantErrors.FeatureUsageEnded;

            if (featureUsage.Value.Used + request.QuestionsNumber > featureUsage.Value.Limit)
                return TenantErrors.FeatureUsageEnded;

            var metadata = await _quizRepository.GetQuizMetadata(request.ItemId, request.CourseId, request.ModuleId, subDomain!, cancellationToken);
            if (metadata is null)
                return ModuleItemErrors.ModuleItemNotFound;

            var quiz = await _quizRepository.GetQuizAsync(request.ItemId, cancellationToken);

            var payload = new AiPayload
            (
                request.Prompt,
                metadata,
                request.Difficulty.ToString().ToLower(),
                request.Type.ToString().ToLower(),
                request.QuestionsNumber
            );

            var endpoint = _options.QuestionEndPoint;
            var result = await _externalService.CallExternalServiceAsync<AiPayload, IEnumerable<AiResponse>>(endpoint, payload, cancellationToken);
            if (result is null)
                throw new Exception();

            var category = await _questionRepository.GetQuestionCategory(TenantMemberConstants.GeneralQuestionCategory, subDomain!, cancellationToken);
            var lastOrder = await _questionRepository.GetLastQuestionOrderAsync(request.ItemId, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var quizQuestions = _mapper.Map<List<QuizQuestion>>(result);
                var order = lastOrder + 1;
                foreach (var quizQuestion in quizQuestions)
                {
                    quizQuestion.QuizId = request.ItemId;
                    quizQuestion.Order = order++;
                    quizQuestion.Question.QuestionCategoryId = category!.Id;
                    quizQuestion.Question.TenantId = tenantId;
                }
                await _questionRepository.AddQuestionsToQuiz(quizQuestions, cancellationToken);
                quiz.TotalMarks += result.Sum(q => q.Marks);
                await _tenantRepository.IncreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.AI_CREDITS, cancellationToken, request.QuestionsNumber);
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