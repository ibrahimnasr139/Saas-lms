using Application.Features.Public.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetOrder
{
    internal sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OneOf<OrderDto, Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetOrderQueryHandler(HybridCache hybridCache, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<OrderDto, Error>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
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

            var order = await _orderRepository.GetStudentOrderAsync(request.OrderId, session.StudentId, subDomain, cancellationToken);
            if (order is null)
                return OrderErrors.OrderNotFound;
            return order;
        }
    }
}
