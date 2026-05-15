using Application.Contracts.Externals;
using Application.Features.Tenants.Dtos;
using Infrastructure.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.Tenants.Commands.GenerateDescription
{
    internal sealed class GenerateDescriptionCommandHandler : IRequestHandler<GenerateDescriptionCommand, OneOf<GenerateDescriptionResponse, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IExternalService _externalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly AiOptions _options;
        public GenerateDescriptionCommandHandler(ITenantMemberRepository tenantMemberRepository, ICourseRepository courseRepository,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor, IExternalService externalService,
            IQuizRepository quizRepository, IQuestionRepository questionRepository, ITenantRepository tenantRepository,
            IOptions<AiOptions> options, IPlanRepository planRepository, IModuleRepository moduleRepository)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _courseRepository = courseRepository;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _externalService = externalService;
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _tenantRepository = tenantRepository;
            _planRepository = planRepository;
            _moduleRepository = moduleRepository;
            _options = options.Value;
        }
        public async Task<OneOf<GenerateDescriptionResponse, Error>> Handle(GenerateDescriptionCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var featureUsage = await _planRepository.GetFeatureUsageInfoAsync(tenantId, FeatureConstants.AI_CREDITS, cancellationToken);
            if (featureUsage is null)
                return TenantErrors.FeatureUsageEnded;

            if (featureUsage.Value.Used + 1 > featureUsage.Value.Limit)
                return TenantErrors.FeatureUsageEnded;

            var context = new ContextDto();
            switch (request.Context)
            {
                case DescriptionType.Course:
                {
                    context.Course = new ContextCourseDto
                    {
                        Title = request.Title
                    };
                    break;
                }
                case DescriptionType.Module:
                {
                    var course = await _courseRepository.GetCourseTitleAndLevelAsync(request.Metadata.CourseId!.Value, subDomain!, cancellationToken);
                    if (course is null)
                        return CourseErrors.CourseNotFound;

                    context.Course = new ContextCourseDto
                    {
                        Title = course.Value.Title,
                        Level = course.Value.Level
                    };
                    context.Module = new ContextTitle { Title = request.Title };
                    break;
                }
                case DescriptionType.Lesson:
                case DescriptionType.Quiz:
                case DescriptionType.Assignment:
                {
                    var course = await _courseRepository.GetCourseTitleAndLevelAsync(request.Metadata.CourseId!.Value, subDomain!, cancellationToken);
                    if (course is null)
                        return CourseErrors.CourseNotFound;

                    var moduleTitle = await _moduleRepository.GetModuleTitleAsync(request.Metadata.ModuleId!.Value, subDomain!, cancellationToken);
                    if (moduleTitle is null)
                        return ModuleErrors.ModuleNotFound;

                    context.Course = new ContextCourseDto
                    {
                        Title = course.Value.Title,
                        Level = course.Value.Level
                    };
                    context.Module = new ContextTitle { Title = moduleTitle };

                    var contextTitle = new ContextTitle { Title = request.Title };
                    if (request.Context == DescriptionType.Lesson)
                        context.Lesson = contextTitle;
                    else if (request.Context == DescriptionType.Quiz)
                        context.Quiz = contextTitle;
                    else
                        context.Assignment = contextTitle;
                    break;
                }
            }

            var payload = new GenerateDescriptionRequest
            {
                Type = request.Context,
                Context = context
            };

            var endpoint = _options.GenerateDescriptionEndPoint;
            var result = await _externalService.CallExternalServiceAsync<GenerateDescriptionRequest, GenerateDescriptionResponse>(endpoint, payload, cancellationToken);
            if (result is null)
                throw new Exception();

            await _tenantRepository.IncreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.AI_CREDITS, cancellationToken);
            return result;
        }
    }
}