using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.CreateQuestion
{
    internal sealed class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, OneOf<SuccessDto, Error>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly ISubscriptionRepository _subscriptionRepository;
        public CreateQuestionCommandHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, IMapper mapper, ISubscriptionRepository subscriptionRepository)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
            {
                return TenantErrors.NotSubscribed;
            }
            var isFoundCategory = await _questionRepository.IsFoundCategory(request.Category, cancellationToken);
            if (!isFoundCategory)
            {
                return QuestionErrors.CategoryNotFound;
            }
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var question = _mapper.Map<Question>(request, opt => opt.AfterMap((src, dest) => dest.TenantId = tenantId));
            var id = await _questionRepository.CreateQuestion(question, cancellationToken);
            return new SuccessDto
            {
                Id = id.ToString(),
                Message = $"{nameof(Module)} {SuccessConstants.ItemCreated}"
            };
        }
    }
}
