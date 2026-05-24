using Application.Contracts.Files;
using Application.Features.Files.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Files.Commands.CreateUpload
{
    internal class CreateUploadCommandHandler : IRequestHandler<CreateUploadCommand, OneOf<CreateUploadDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlanRepository _planRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        private const int OverFlowSizeMB = 20;

        public CreateUploadCommandHandler(IHttpContextAccessor httpContextAccessor, IPlanRepository planRepository,
            ISubscriptionRepository subscriptionRepository, ITenantRepository tenantRepository, IFileService fileService,
            IFileRepository fileRepository, ICurrentUserId currentUserId, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _planRepository = planRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantRepository = tenantRepository;
            _fileService = fileService;
            _fileRepository = fileRepository;
            _currentUserId = currentUserId;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneOf<CreateUploadDto, Error>> Handle(CreateUploadCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);
            var planPricingId = await _subscriptionRepository.GetPlanPricingIdAsync(tenantId, cancellationToken);
            var planId = await _planRepository.GetPlanIdAsync(planPricingId, cancellationToken);
            var featureId = await _planRepository.GetFeatureIdAsync(FeatureConstants.VIDEO_STORAGE_GB, cancellationToken);
            var planFeatureId = await _planRepository.GetPlanFeatureIdByFeatureIdAsync(planId, featureId, cancellationToken);

            var limitValueGB = await _planRepository.GetFeatureLimitAsync(planFeatureId, cancellationToken);
            var limitMB = limitValueGB * 1024;

            var usedBytes = await _tenantRepository.GetPlanFeatureUsageAsync(planFeatureId, tenantId, cancellationToken);
            var usedMB = Math.Max(0, usedBytes / (1024 * 1024));
            var requestMB = request.Size / (1024 * 1024);
            var totalAfterUpload = usedMB + requestMB - OverFlowSizeMB;

            if (totalAfterUpload > limitMB)
                return FileErrors.UploadFailed;

            var credentials = await _fileService.CreateUploadCredentialsAsync(request.Title, request.Size, cancellationToken);
            if (credentials == null)
                return FileErrors.UploadFailed;

            var newFile = new Domain.Entites.File
            {
                Id = credentials.VideoId,
                Name = request.Title,
                Size = requestMB,
                Type = Domain.Enums.FileType.Video,
                Status = Domain.Enums.FileStatus.Pending,
                Url = credentials.EmbedUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "Duration", request.Duration.ToString() }
                },
                TenantId = tenantId,
                UploadedById = userId
            };
            await _fileRepository.CreateAsync(newFile, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return credentials;
        }
    }
}
