using Application.Features.Public.Dtos;
using Application.Features.Website.Commands.BulkOrderAction;
using Application.Features.Website.Dtos;
using Domain.Enums;

namespace Application.Contracts.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order, CancellationToken cancellationToken);
        Task<List<TenantOrderDto>> GetTenantOrdersAsync(string subDomain, CancellationToken cancellationToken);
        Task<TenantOrderStatisticsDto> GetTenantOrderStatisticsAsync(int tenantId, CancellationToken cancellationToken);
        Task<bool> ApproveOrderWithEnrollmentAsync(int orderId, int tenantId, string actor, Enrollment enrollment, StudentSubscription subscription, CancellationToken cancellationToken);
        Task<bool> DeclineOrderAsync(int orderId, int tenantId, string actor, string? reason, CancellationToken cancellationToken);
        Task<bool> BulkOrderActionAsync(int tenantId, string actor, BulkOrderActionCommand request, CancellationToken cancellationToken);
        Task<Order?> GetOrderAsync(int tenantId, int orderId, CancellationToken cancellationToken);
        Task<OrderDto?> GetStudentOrderAsync(int orderId, int studentId, string subDomain, CancellationToken cancellationToken);
        Task<OrderStatus> GetOrderStatusAsync(int orderId, CancellationToken cancellationToken);
        Task<Order?> GetOrderByIdAsync(int orderId, int studentId, string subDomain, CancellationToken cancellationToken);
    }
}