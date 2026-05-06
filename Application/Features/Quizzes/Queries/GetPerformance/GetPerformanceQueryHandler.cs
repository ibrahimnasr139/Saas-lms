using Application.Features.Quizzes.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Quizzes.Queries.GetPerformance
{
    internal sealed class GetPerformanceQueryHandler : IRequestHandler<GetPerformanceQuery, OneOf<PerformanceDto, Error>>
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IAttemptRepository _attemptRepository;
        public GetPerformanceQueryHandler(IHttpContextAccessor httpContextAccessor, IAttemptRepository attemptRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _attemptRepository = attemptRepository;
        }
        public async Task<OneOf<PerformanceDto, Error>> Handle(GetPerformanceQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var quiz = await _moduleItemRepository.GetQuizAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (quiz is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return new PerformanceDto
            {
                AttemptsOverTime = await _attemptRepository.GetAttemptsOverTimeAsync(request.ItemId, cancellationToken),
                GradesDistribution = await _attemptRepository.GetAttemptGradeDistributionAsync(request.ItemId, cancellationToken),
                MostDifficultQuestions = await _attemptRepository.GetAttemptMostDifficultQuestionsAsync(request.ItemId, cancellationToken)
            };
        }
    }
}
