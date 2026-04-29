using Application.Features.Public.Dtos;
using Application.Features.TenantOrders.Commands.BulkOrderAction;
using Application.Features.TenantOrders.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Infrastructure.Constants;

namespace Infrastructure.Repositories
{
    internal sealed class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }
        public async Task<List<TenantOrderDto>> GetTenantOrdersAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.TenantId == tenantId)
                .ProjectTo<TenantOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<TenantOrderStatisticsDto> GetTenantOrderStatisticsAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.TenantId == tenantId)
                .GroupBy(o => o.TenantId)
                .Select(g => new TenantOrderStatisticsDto
                {
                    TotalOrders = g.Count(),
                    PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                    ApprovedOrders = g.Count(o => o.Status == OrderStatus.Approved),
                    DeclinedOrders = g.Count(o => o.Status == OrderStatus.Declined),
                    TotalRevenue = g.Where(o => o.Status == OrderStatus.Approved).Sum(o => o.PricePaid)
                }).FirstOrDefaultAsync(cancellationToken) ?? new TenantOrderStatisticsDto();
        }
        public async Task<bool> ApproveOrderWithEnrollmentAsync(int orderId, int tenantId, string actor, Enrollment enrollment, StudentSubscription subscription, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenantId, cancellationToken);
                if (order is null || order.Status != OrderStatus.Pending)
                    return false;

                order.Status = OrderStatus.Approved;
                order.ApprovedAt = DateTime.UtcNow;
                order.OrderTimeLines.Add(new OrderTimeLine
                {
                    Description = OrderTimeLineConstants.OrderTimeLineApproved,
                    Actor = actor,
                    Type = OrderTimeLineType.Approved,
                    OrderId = order.Id
                });

                await _context.Enrollments.AddAsync(enrollment, cancellationToken);
                await _context.StudentSubscriptions.AddAsync(subscription, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
        }
        public async Task<bool> DeclineOrderAsync(int orderId, int tenantId, string actor, string? reason, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenantId);
            if (order == null || order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Declined;
            order.DeclinedAt = DateTime.UtcNow;
            order.RejectionReason = reason;
            order.OrderTimeLines.Add(new OrderTimeLine
            {
                Description = OrderTimeLineConstants.OrderTimeLineDeclined,
                Actor = actor,
                Type = OrderTimeLineType.Declined,
                OrderId = order.Id
            });
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> BulkOrderActionAsync(int tenantId, string actor, BulkOrderActionCommand request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .Where(o => request.OrderIds.Contains(o.Id) && o.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            if (orders.Count == 0)
                return false;

            foreach (var order in orders)
            {
                if (order.Status != OrderStatus.Pending)
                    continue;
                if (request.Action == OrderStatus.Approved)
                {
                    order.Status = OrderStatus.Approved;
                    order.ApprovedAt = DateTime.UtcNow;
                    order.OrderTimeLines.Add(new OrderTimeLine
                    {
                        Description = OrderTimeLineConstants.OrderTimeLineApproved,
                        Actor = actor,
                        Type = OrderTimeLineType.Approved,
                        OrderId = order.Id
                    });
                }
                else if (request.Action == OrderStatus.Declined)
                {
                    order.Status = OrderStatus.Declined;
                    order.DeclinedAt = DateTime.UtcNow;
                    order.RejectionReason = request.Reason;
                    order.OrderTimeLines.Add(new OrderTimeLine
                    {
                        Description = OrderTimeLineConstants.OrderTimeLineDeclined,
                        Actor = actor,
                        Type = OrderTimeLineType.Declined,
                        OrderId = order.Id
                    });
                }
            }
            _context.Orders.UpdateRange(orders);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<Order?> GetOrderAsync(int tenantId, int orderId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Course)
                    .ThenInclude(c => c.CreatedBy)
                .Where(o => o.Id == orderId && o.TenantId == tenantId)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<OrderDto?> GetStudentOrderAsync(int orderId, int studentId, string subDomain, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Course)
                .Where(o => o.Id == orderId && o.StudentId == studentId)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<OrderStatus> GetOrderStatusAsync(int orderId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => o.Status)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Order?> GetOrderByIdAsync(int orderId, int studentId, string subDomain, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.StudentId == studentId && o.Tenant.SubDomain == subDomain, cancellationToken);
        }
    }
}