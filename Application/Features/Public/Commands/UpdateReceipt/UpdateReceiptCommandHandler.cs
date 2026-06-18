using Application.Features.Website.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Commands.UpdateReceipt
{
    internal sealed class UpdateReceiptCommandHandler : IRequestHandler<UpdateReceiptCommand, OneOf<TenantOrderResponse, Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReceiptCommandHandler(IStudentRepository studentRepository, HybridCache hybridCache, IOrderRepository orderRepository,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _studentRepository = studentRepository;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<TenantOrderResponse, Error>> Handle(UpdateReceiptCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, session.StudentId, subDomain, cancellationToken);
            if (order is null)
                return OrderErrors.OrderNotFound;

            var orderStatus = await _orderRepository.GetOrderStatusAsync(request.OrderId, cancellationToken);
            if (orderStatus == OrderStatus.approved)
                return OrderErrors.CanNotUpdatedApprovedOrder;
            else if (orderStatus == OrderStatus.declined)
                return OrderErrors.CanNotUpdatedDeclinedOrder;

            order.PaymentReference = request.PaymentReference;
            order.PaymentProof = request.PaymentProof;
            order.OrderTimeLines.Add(new OrderTimeLine
            {
                OrderId = order.Id,
                Description = "تم رفع إثبات الدفع",
                Type = OrderTimeLineType.paymentuploaded,
                Actor = await _studentRepository.GetStuentNameByIdAsync(session.StudentId, cancellationToken)
            });
            await _unitOfWork.SaveAsync(cancellationToken);
            return new TenantOrderResponse { Message = MessagesConstants.OrderUpdated };
        }
    }
}