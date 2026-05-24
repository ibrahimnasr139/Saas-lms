using Application.Features.Quizzes.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Quizzes.Queries.GetOverview
{
    internal sealed class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, OneOf<OverviewDto, Error>>
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetOverviewQueryHandler(IHttpContextAccessor httpContextAccessor, IQuizRepository quizRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _quizRepository = quizRepository;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<OverviewDto, Error>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var quiz = await _moduleItemRepository.GetQuizAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (quiz is null)
                return ModuleItemErrors.ModuleItemNotFound;
            return await _quizRepository.GetQuizOverview(request.ItemId, cancellationToken) ?? new OverviewDto();
        }
    }
}