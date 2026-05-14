using Application.Features.TenantOrders.Dtos;
using Application.Helpers;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantOrders.Commands.ApproveOrder
{
    internal sealed class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, OneOf<TenantOrderResponse, Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly HybridCache _hybridCache;
        private readonly IStudentRepository _studentRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IEmailSender _emailSender;

        public ApproveOrderCommandHandler(IOrderRepository orderRepository, ITenantRepository tenantRepository, IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor, ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository,
            ISubscriptionRepository subscriptionRepository, HybridCache hybridCache, IStudentRepository studentRepository,
            IModuleRepository moduleRepository, IModuleItemRepository moduleItemRepository)
        {
            _orderRepository = orderRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
            _subscriptionRepository = subscriptionRepository;
            _hybridCache = hybridCache;
            _studentRepository = studentRepository;
            _moduleRepository = moduleRepository;
            _moduleItemRepository = moduleItemRepository;
            _emailSender = emailSender;
        }
        public async Task<OneOf<TenantOrderResponse, Error>> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var userId = _currentUserId.GetUserId();
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_ORDERS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var order = await _orderRepository.GetOrderAsync(tenantId, request.OrderId, cancellationToken);
            if (order is null)
                return OrderErrors.OrderApproveFailed;

            var currentTenantMember = await _tenantMemberRepository.GetCurrentTenantMemberAsync(userId, cancellationToken);
            var actor = $"{currentTenantMember!.FirstName} {currentTenantMember!.LastName}";

            int? firstModuleId = await _moduleRepository.GetFirstModuleIdAsync(order.CourseId, cancellationToken);
            int? firstModuleItemId = await _moduleItemRepository.GetFirstModuleItemAsync(firstModuleId, cancellationToken);

            var newEnrollment = new Enrollment
            {
                CourseId = order.CourseId,
                StudentId = order.StudentId,
                EnrollmentType = EnrollmentType.Purchased,
                TenantId = tenantId,
                CurrentModuleId = firstModuleId,
                CurrentModuleItemId = firstModuleItemId,
                OrderId = order.Id
            };

            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            var newSubscription = new StudentSubscription
            {
                StartDate = now,
                EndDate = CalculateEndDate(order.Course, now),
                StudentId = order.StudentId,
                CourseId = order.CourseId,
                TenantId = tenantId
            };

            var result = await _orderRepository.ApproveOrderWithEnrollmentAsync(
                request.OrderId, tenantId,
                actor, newEnrollment,
                newSubscription, cancellationToken
            );
            if (!result)
                return OrderErrors.OrderApproveFailed;

            var student = await _studentRepository.GetStudentAsync(order.StudentId, cancellationToken);
            var studentEmail = await _studentRepository.GetStudentEmailAsync(student!.UserId, cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.ApproveOrderTemplate,
                new Dictionary<string, string>
                {
                    { "{{StudentName}}", $"{student!.User.FirstName} {student!.User.LastName}" },
                    { "{{CourseName}}", order.Course.Title },
                    { "{{TeacherName}}", $"{order.Course.CreatedBy.FirstName} {order.Course.CreatedBy.LastName}" },
                    { "{{CourseLink}}", $"{EmailConstants.CourseLink}/{order.CourseId}" },
                }
            );
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(studentEmail, EmailConstants.OrderApprovalSubject, emailBody));
            await _hybridCache.RemoveAsync($"{CacheKeysConstants.StudentCoursesKey}_{order.StudentId}", cancellationToken);
            return new TenantOrderResponse { Message = MessagesConstants.OrderApproved };
        }
        private static DateOnly? CalculateEndDate(Course course, DateOnly now)
        {
            return course.PricingType switch
            {
                PricingType.PerSemester => now.AddMonths(3),
                PricingType.Subscription => course.BillingCycle switch
                {
                    BillingCycle.monthly => now.AddMonths(1),
                    BillingCycle.annually => now.AddMonths(8),
                    _ => null
                },
                _ => null
            };
        }
    }
}