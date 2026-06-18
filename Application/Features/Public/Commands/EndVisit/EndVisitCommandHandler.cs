using Microsoft.AspNetCore.Http;

namespace Application.Features.Public.Commands.EndVisit
{
    internal sealed class EndVisitCommandHandler : IRequestHandler<EndVisitCommand, OneOf<bool, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantPageVisitRepository _tenantPageVisitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EndVisitCommandHandler(IHttpContextAccessor httpContextAccessor, ITenantPageVisitRepository tenantPageVisitRepository,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantPageVisitRepository = tenantPageVisitRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(EndVisitCommand request, CancellationToken cancellationToken)
        {
            string subDomain = string.Empty;
            var httpRequest = _httpContextAccessor.HttpContext!.Request;
            var origin = httpRequest.Headers["Origin"].ToString();
            if (!string.IsNullOrEmpty(origin) && Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                subDomain = uri.Host.Split('.')[0];
            else
                subDomain = httpRequest.Host.Host.Split(".")[0];

            var pageVisit = await _tenantPageVisitRepository.GetTenantPageVisitAsync(subDomain, request.PageUrl, request.VisitorId, cancellationToken);
            if (pageVisit is null)
                return TenantWebsiteErrors.TenantPageNotFound;

            pageVisit.DurationSecond = request.DurationSecond;
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}