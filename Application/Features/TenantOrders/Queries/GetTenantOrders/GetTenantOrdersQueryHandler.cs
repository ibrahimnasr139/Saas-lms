using Application.Features.TenantOrders.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantOrders.Queries.GetTenantOrders
{
    internal sealed class GetTenantOrdersQueryHandler : IRequestHandler<GetTenantOrdersQuery, List<TenantOrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetTenantOrdersQueryHandler(IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TenantOrderDto>> Handle(GetTenantOrdersQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _orderRepository.GetTenantOrdersAsync(subDomain!, cancellationToken);
        }
    }
}
