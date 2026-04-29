using Application.Contracts.Repositories;
using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsite.Commands.CreateTenantPage
{
    internal sealed class CreateTenantPageCommandHandler : IRequestHandler<CreateTenantPageCommand, OneOf<TenantPageResponse, Error>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTenantPageCommandHandler(ITenantPageRepository tenantWebsiteRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subscriptionRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<TenantPageResponse, Error>> Handle(CreateTenantPageCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var urlExists = await _tenantWebsiteRepository.UrlExistsAsync(tenantId, request.Url, cancellationToken);
            if (urlExists)
                return TenantWebsiteErrors.PageUrlAlreadyExists;

            await _tenantWebsiteRepository.CreateTenantPageAsync(request, tenantId, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new TenantPageResponse { Message = MessagesConstants.TenantWebsiteCreated };
        }
    }
}