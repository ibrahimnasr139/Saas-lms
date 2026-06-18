using Application.Features.Website.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Website.Commands.DeleteTenantPage
{
    internal sealed class DeleteTenantPageCommandHandler : IRequestHandler<DeleteTenantPageCommand, OneOf<TenantPageResponse, Error>>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteTenantPageCommandHandler(ITenantPageRepository tenantWebsiteRepository, ITenantRepository tenantRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<TenantPageResponse, Error>> Handle(DeleteTenantPageCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var result = await _tenantWebsiteRepository.DeleteTenantPageAsync(tenantId, request.PageId, cancellationToken);
            if (result == 0)
                return TenantWebsiteErrors.TenantPageNotFound;
            return new TenantPageResponse { Message = MessagesConstants.TenantWebsiteDeleted };
        }
    }
}
