using Application.Contracts.Files;
using Application.Features.Files.Dtos;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Application.Features.Files.Commands.CreateUpload
{
    internal class CreateUploadCommandHandler : IRequestHandler<CreateUploadCommand, OneOf<CreateUploadDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlanRepository _planRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private const long OverflowBytes = 20L * 1024 * 1024;

        public CreateUploadCommandHandler(IHttpContextAccessor httpContextAccessor, IPlanRepository planRepository,
            ITenantRepository tenantRepository, IFileService fileService, IUnitOfWork unitOfWork, ICurrentUserId currentUserId,
            IFileRepository fileRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _planRepository = planRepository;
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
            var featureUsage = await _planRepository.GetFeatureUsageInfoAsync(tenantId, FeatureConstants.VIDEO_STORAGE_GB, cancellationToken);
            if (featureUsage is null)
                return FileErrors.UploadFailed;

            var (usedBytes, limitGB) = featureUsage.Value;
            var limitBytes = limitGB * 1024L * 1024L * 1024L;
            var totalAfterUploadBytes =  Math.Max(0, usedBytes + request.Size - OverflowBytes);
            if (totalAfterUploadBytes > limitBytes)
                return FileErrors.UploadFailed;

            var credentials = await _fileService.CreateUploadCredentialsAsync(request.Title, request.Size, cancellationToken);
            if (credentials is null)
                return FileErrors.UploadFailed;

            var newFile = new Domain.Entites.File
            {
                Id = credentials.VideoId,
                Name = request.Title,
                Size = request.Size,
                Type = Domain.Enums.FileType.Video,
                Status = Domain.Enums.FileStatus.Pending,
                Url = credentials.EmbedUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "duration", request.Duration.ToString(CultureInfo.InvariantCulture) }
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
