using Application.Features.TenantOrders.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantOrders.Commands.DeclineOrder
{
    internal sealed class DeclineOrderCommandHandler : IRequestHandler<DeclineOrderCommand, OneOf<TenantOrderResponse, Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserId _currentUserId;
        private readonly ITenantMemberRepository _tenantMemberRepository;
        public DeclineOrderCommandHandler(IOrderRepository orderRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ICurrentUserId currentUserId, ITenantMemberRepository tenantMemberRepository)
        {
            _orderRepository = orderRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserId = currentUserId;
            _tenantMemberRepository = tenantMemberRepository;
        }
        public async Task<OneOf<TenantOrderResponse, Error>> Handle(DeclineOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var isPermitted = await _tenantMemberRepository.IsPermittedMember(userId, PermissionConstants.MANAGE_ORDERS, cancellationToken);
            if (!isPermitted)
                return MemberErrors.NotAllowed;

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var currentTenantMember = await _tenantMemberRepository.GetCurrentTenantMemberAsync(userId, cancellationToken);
            var actor = $"{currentTenantMember!.FirstName} {currentTenantMember!.LastName}";

            var result = await _orderRepository.DeclineOrderAsync(request.OrderId, tenantId, actor, request.Reason, cancellationToken);
            if (!result)
                return OrderErrors.OrderDeclineFailed;

            return new TenantOrderResponse { Message = MessagesConstants.OrderDeclined };
        }
    }
}
