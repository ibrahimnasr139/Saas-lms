using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.CreateQuestionCategory
{
    internal sealed class CreateQuestionCategoryCommandHandler : IRequestHandler<CreateQuestionCategoryCommand, OneOf<bool, Error>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        public CreateQuestionCategoryCommandHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor, ITenantRepository tenantRepository)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateQuestionCategoryCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            bool isUniqueCategory = await _questionRepository.IsUniqueCategory(request.Title, subDomain!, cancellationToken);
            if (!isUniqueCategory)
            {
                return QuestionErrors.CategoryAlreadyExists;
            }
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            await _questionRepository.CreateQuestionCategory(new QuestionCategory { Title = request.Title, TenantId = tenantId }, cancellationToken);
            return true;
        }
    }
}
