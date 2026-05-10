using Application.Helpers;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Commands.UpdateSettings
{
    internal sealed class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, OneOf<SuccessDto, Error>>
    {
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _hybridCache;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IEmailSender _emailSender;

        public UpdateSettingsCommandHandler(ITenantMemberRepository tenantMemberRepository, ICurrentUserId currentUserId,
            ISubscriptionRepository subscriptionRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork, HybridCache hybridCache,
            IEnrollmentRepository enrollmentRepository, IEmailSender emailSender)
        {
            _tenantMemberRepository = tenantMemberRepository;
            _currentUserId = currentUserId;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
            _hybridCache = hybridCache;
            _enrollmentRepository = enrollmentRepository;
            _emailSender = emailSender;
        }
        public async Task<OneOf<SuccessDto, Error>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_MODULE_ITEMS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subdomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var moduleItem = await _moduleItemRepository.GetItemConditions(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (moduleItem is null)
                return ModuleItemErrors.ModuleItemNotFound;

            var previousStatus = moduleItem.Status;
            moduleItem.Conditions.Clear();
            _mapper.Map(request, moduleItem);
            var mappedConditions = _mapper.Map<IEnumerable<ModuleItemCondition>>(request.Conditions);
            foreach (var condition in mappedConditions)
            {
                condition.ModuleItemId = moduleItem.Id;
                moduleItem.Conditions.Add(condition);
            }
            await _unitOfWork.SaveAsync(cancellationToken);
            var cacheKey = $"{CacheKeysConstants.CourseKey}_{request.CourseId}_{CacheKeysConstants.ItemKey}_{request.ItemId}";
            await _hybridCache.RemoveAsync(cacheKey, cancellationToken);

            if (request.Status == CourseStatus.Published && previousStatus == CourseStatus.Draft)
            {
                var itemType = moduleItem.Type switch
                {
                    ModuleItemType.Lesson => "درس",
                    ModuleItemType.Assignment => "واجب",
                    ModuleItemType.Quiz => "كويز",
                    _ => "محتوى"
                };

                var template = moduleItem.Type switch
                {
                    ModuleItemType.Lesson => EmailConstants.NewLessonNotificationTemplate,
                    ModuleItemType.Assignment => EmailConstants.NewAssignmentNotificationTemplate,
                    ModuleItemType.Quiz => EmailConstants.NewQuizNotificationTemplate,
                    _ => EmailConstants.NewLessonNotificationTemplate
                };

                var students = await _enrollmentRepository.GetEnrolledStudentsForNotificationAsync(
                    request.CourseId, moduleItem.Title, itemType,
                    moduleItem.Assignment?.DueDate,
                    moduleItem.Quiz?.StartDate,
                    moduleItem.Quiz?.EndDate,
                    cancellationToken);

                foreach (var student in students)
                {
                    var placeholders = new Dictionary<string, string>
                    {
                        { "{{StudentName}}", student.StudentName },
                        { "{{ItemTitle}}", student.ItemTitle },
                        { "{{CourseTitle}}", student.CourseTitle },
                        { "{{DueDate}}", student.DueDate.HasValue ? student.DueDate.Value.ToString("yyyy-MM-dd HH:mm") + " UTC" : "-" },
                        { "{{StartDate}}", student.StartDate.HasValue ? student.StartDate.Value.ToString("yyyy-MM-dd HH:mm") + " UTC" : "-" },
                        { "{{EndDate}}", student.EndDate.HasValue ? student.EndDate.Value.ToString("yyyy-MM-dd HH:mm") + " UTC" : "-" },
                        { "{{DashboardUrl}}", $"{EmailConstants.CourseLink}/{request.CourseId}" },
                    };

                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(template, placeholders);
                    var subject = $"تمت إضافة {itemType} جديد في {student.CourseTitle}";

                    BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(student.StudentEmail, subject, emailBody));
                }
            }
            return new SuccessDto
            {
                Id = moduleItem.Id.ToString(),
                Message = $"{nameof(ModuleItem)} {SuccessConstants.ItemUpdated}"
            };
        }
    }
}
