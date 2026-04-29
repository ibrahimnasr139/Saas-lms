using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Files.Commands.VideoStatus
{
    internal sealed class VideoStatusCommandHandler : IRequestHandler<VideoStatusCommand, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlanRepository _planRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VideoStatusCommandHandler(IHttpContextAccessor httpContextAccessor, IPlanRepository planRepository, IUnitOfWork unitOfWork,
            ISubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository, IFileRepository fileRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _planRepository = planRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantRepository = tenantRepository;
            _fileRepository = fileRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(VideoStatusCommand request, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetFileByIdAsync(request.Id, cancellationToken);
            if (file == null)
                return Unit.Value;

            if (request.Status == FileStatus.Processing.ToString())
            {
                file.Status = FileStatus.Processing;
                if (request.Size.HasValue && request.Size.Value > 0)
                {
                    var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
                    var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
                    var planPricingId = await _subscriptionRepository.GetPlanPricingIdAsync(tenantId, cancellationToken);
                    var planId = await _planRepository.GetPlanIdAsync(planPricingId, cancellationToken);
                    var featureId = await _planRepository.GetVideoStorageFeatureIdAsync(cancellationToken);
                    var planFeatureId = await _planRepository.GetPlanFeatureIdByFeatureIdAsync(planId, featureId, cancellationToken);

                    await _tenantRepository.InCreasePlanFeatureUsageAsync(tenantId, planFeatureId, request.Size.Value, cancellationToken);
                }
            }
            else
                await _fileRepository.DeleteFileAsync(file, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
