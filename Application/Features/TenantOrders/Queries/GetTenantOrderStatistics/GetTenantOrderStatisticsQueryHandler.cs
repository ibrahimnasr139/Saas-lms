using Application.Features.TenantOrders.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantOrders.Queries.GetTenantOrderStatistics
{
    internal sealed class GetTenantOrderStatisticsQueryHandler : IRequestHandler<GetTenantOrderStatisticsQuery, TenantOrderStatisticsDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetTenantOrderStatisticsQueryHandler(IOrderRepository orderRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TenantOrderStatisticsDto> Handle(GetTenantOrderStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            return await _orderRepository.GetTenantOrderStatisticsAsync(tenantId, cancellationToken);
        }
    }
}
