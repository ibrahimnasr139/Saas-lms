using Application.Features.Questions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Queries.GetCategories
{
    internal sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<QuestionCategoryDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetCategoriesQueryHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<QuestionCategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _questionRepository.GetQuestionWithCategory(subDomain!, cancellationToken);
        }
    }
}
