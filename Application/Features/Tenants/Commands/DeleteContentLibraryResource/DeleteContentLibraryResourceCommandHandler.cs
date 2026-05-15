using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Commands.DeleteContentLibraryResource
{
    internal class DeleteContentLibraryResourceCommandHandler :
            IRequestHandler<DeleteContentLibraryResourceCommand, OneOf<DeleteContentLibraryResourceResponse, Error>>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlanRepository _planRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITenantRepository _tenantRepository;

        public DeleteContentLibraryResourceCommandHandler(IHttpContextAccessor httpContextAccessor, IPlanRepository planRepository,
            ISubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository, IFileRepository fileRepository,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _planRepository = planRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantRepository = tenantRepository;
            _fileRepository = fileRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<DeleteContentLibraryResourceResponse, Error>> Handle(DeleteContentLibraryResourceCommand request,
            CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetFileByIdAsync(request.FileId, cancellationToken);
            if (file == null)
                return FileErrors.NotFound;

            var size = file.Size;
            await _fileRepository.DeleteFileAsync(file, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var planPricingId = await _subscriptionRepository.GetPlanPricingIdAsync(tenantId, cancellationToken);
            var planId = await _planRepository.GetPlanIdAsync(planPricingId, cancellationToken);
            var featureId = await _planRepository.GetFeatureIdAsync(FeatureConstants.VIDEO_STORAGE_GB, cancellationToken);
            var planFeatureId = await _planRepository.GetPlanFeatureIdByFeatureIdAsync(planId, featureId, cancellationToken);

            await _tenantRepository.DeCreasePlanFeatureUsageAsync(tenantId, planFeatureId, size, cancellationToken);
            return new DeleteContentLibraryResourceResponse("File deleted successfully");
        }
    }
}
