using Application.Features.Questions.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Queries.GetStatistics
{
    internal sealed class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, QuestionStatisticsDto>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetStatisticsQueryHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<QuestionStatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var totalQuestions = await _questionRepository.GetTotalQuestions(subDomain!, cancellationToken);
            var usedQuestions = await _questionRepository.GetUsedQuestions(subDomain!, cancellationToken);
            var newThisWeek = await _questionRepository.GetWeekQuestions(subDomain!, cancellationToken);
            var categories = await _questionRepository.GetQuestionCategories(subDomain!, cancellationToken);
            var questionsByType = await _questionRepository.GetQuestionsByType(subDomain!, cancellationToken);
            return new QuestionStatisticsDto
            {
                TotalQuestions = totalQuestions,
                UsedQuestions = usedQuestions,
                NewThisWeek = newThisWeek,
                Categories = categories,
                QuestionsByType = questionsByType
            };
        }
    }
}
