using Application.Features.Website.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Commands.BulkOrderAction
{
    internal sealed class BulkOrderActionCommandHandler : IRequestHandler<BulkOrderActionCommand, OneOf<TenantOrderResponse, Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public BulkOrderActionCommandHandler(IOrderRepository orderRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _orderRepository = orderRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<OneOf<TenantOrderResponse, Error>> Handle(BulkOrderActionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            if (request.Action == OrderStatus.approved)
            {
                var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
                if (!isSubscribed)
                    return TenantErrors.NotSubscribed;
            }

            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_ORDERS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var currentTenantMember = await _tenantMemberRepository.GetCurrentTenantMemberAsync(userId, cancellationToken);
            var actor = $"{currentTenantMember!.FirstName} {currentTenantMember!.LastName}";

            var result = await _orderRepository.BulkOrderActionAsync(tenantId, actor, request, cancellationToken);
            if (!result)
                return OrderErrors.BulkActionFailed;

            return request.Action == OrderStatus.approved
                ? new TenantOrderResponse { Message = MessagesConstants.OrdersApproved }
                : new TenantOrderResponse { Message = MessagesConstants.OrdersDeclined };
        }
    }
}
