using Application.Features.Public.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Queries.GetTenantPaymentMethods
{
    internal sealed class GetTenantPaymentMethodsQueryHandler : IRequestHandler<GetTenantPaymentMethodsQuery, OneOf<List<PublicPaymentMethodDto>, Error>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HybridCache _hybridCache;

        public GetTenantPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, HybridCache hybridCache)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
        }
        public async Task<OneOf<List<PublicPaymentMethodDto>, Error>> Handle(GetTenantPaymentMethodsQuery request, CancellationToken cancellationToken)
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

            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain, cancellationToken);
            return await _paymentMethodRepository.GetPaymentMethodsByTenantIdAsync(tenantId, cancellationToken);
        }
    }
}