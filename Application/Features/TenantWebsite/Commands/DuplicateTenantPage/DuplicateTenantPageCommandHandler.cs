using Application.Contracts.Repositories;
using Application.Features.TenantWebsite.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantWebsite.Commands.DuplicateTenantPage
{
    internal sealed class DuplicateTenantPageCommandHandler : IRequestHandler<DuplicateTenantPageCommand, OneOf<TenantPageResponse, Error>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DuplicateTenantPageCommandHandler(ITenantPageRepository tenantWebsiteRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<TenantPageResponse, Error>> Handle(DuplicateTenantPageCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var tenantPage = await _tenantWebsiteRepository.GetTenantPageAsync(tenantId, request.PageId, cancellationToken);
            if(tenantPage is null)
                return TenantWebsiteErrors.TenantPageNotFound;

            var duplicatedTenantPage = new TenantPage
            {
                Title = tenantPage.Title ,
                MetaTitle = tenantPage.MetaTitle,
                MetaDescription = tenantPage.MetaDescription,
                Status = tenantPage.Status,
                IsHomePage = tenantPage.IsHomePage,
                TenantId = tenantPage.TenantId,
                PageBlocks = tenantPage.PageBlocks.Select(pb => new PageBlock
                {
                    BlockTypeId = pb.BlockTypeId,
                    Order = pb.Order,
                    Visible = pb.Visible,
                    Props = pb.Props,
                }).ToList()
            };
            await _tenantWebsiteRepository.DuplicateTenantPageAsync(duplicatedTenantPage,cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            duplicatedTenantPage.Url = $"{tenantPage.Url}-{duplicatedTenantPage.Id}";
            await _unitOfWork.SaveAsync(cancellationToken);
            return new TenantPageResponse { Message = MessagesConstants.TenantWebsiteDuplicated };
        }
    }
}
