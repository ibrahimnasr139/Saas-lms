using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Commands.VisitPage
{
    internal sealed class VisitPageCommandHandler : IRequestHandler<VisitPageCommand, OneOf<bool, Error>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantPageVisitRepository _tenantPageVisitRepository;
        private readonly ITenantPageRepository _tenantPageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VisitPageCommandHandler(ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor,
            ITenantPageVisitRepository tenantPageVisitRepository, ITenantPageRepository tenantPageRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
            _tenantPageVisitRepository = tenantPageVisitRepository;
            _tenantPageRepository = tenantPageRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(VisitPageCommand request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var pageIsExist = await _tenantPageRepository.UrlExistsAsync(subDomain, request.PageUrl, cancellationToken);
            if (!pageIsExist)
                return TenantWebsiteErrors.TenantPageNotFound;

            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain, cancellationToken);
            var pageVisit = await _tenantPageVisitRepository.GetByVisitorAndPageAsync(request.VisitorId, tenantId, request.PageUrl, cancellationToken);
            if (pageVisit is null)
            {
                pageVisit = new TenantPageVisit
                {
                    VisitorId = request.VisitorId,
                    DeviceType = request.DeviceType,
                    PageUrl = request.PageUrl,
                    TenantId = tenantId,
                };
                await _tenantPageVisitRepository.AddTenantPageVisitAsync(pageVisit, cancellationToken);
            }
            else
            {
                pageVisit.Views++;
                pageVisit.VisitedAt = DateTime.UtcNow;
                pageVisit.DeviceType = request.DeviceType;
            }
            var tenantPage = await _tenantPageRepository.GetTenantPageByUrlAsync(subDomain, request.PageUrl, cancellationToken);
            tenantPage.Views++;
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}