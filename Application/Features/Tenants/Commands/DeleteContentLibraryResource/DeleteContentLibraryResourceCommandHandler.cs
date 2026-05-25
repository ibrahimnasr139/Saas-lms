using Application.Features.Tenants.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Tenants.Commands.DeleteContentLibraryResource
{
    internal class DeleteContentLibraryResourceCommandHandler : IRequestHandler<DeleteContentLibraryResourceCommand, OneOf<DeleteContentLibraryResourceResponse, Error>>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        public DeleteContentLibraryResourceCommandHandler(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            ITenantRepository tenantRepository, IFileRepository fileRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _fileRepository = fileRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<DeleteContentLibraryResourceResponse, Error>> Handle(DeleteContentLibraryResourceCommand request,
            CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var file = await _fileRepository.GetFileByIdAsync(request.FileId, cancellationToken);
            if (file is null)
                return FileErrors.NotFound;

            var fileSizeBytes = file.Size;
            await _fileRepository.DeleteFileAsync(file, cancellationToken);
            await _tenantRepository.DecreasePlanFeatureUsageByKeyAsync(subDomain!, FeatureConstants.VIDEO_STORAGE_GB, cancellationToken, fileSizeBytes);
            await _unitOfWork.SaveAsync(cancellationToken);
            return new DeleteContentLibraryResourceResponse("File deleted successfully");
        }
    }
}