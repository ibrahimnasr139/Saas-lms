using Application.Contracts.Files;
using Application.Features.Files.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Files.Commands.UploadFile
{
    internal class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, OneOf<UploadFileDto, Error>>
    {
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadFileCommandHandler(IFileService fileService, IFileRepository fileRepository, ICurrentUserId currentUserId,
            IHttpContextAccessor httpContextAccessor, ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _fileRepository = fileRepository;
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneOf<UploadFileDto, Error>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var fileType = _fileService.GetFileType(request.File.ContentType);
            var fileId = Guid.NewGuid().ToString();
            var fileName = request.Name ?? request.File.FileName;
            var path = _fileService.GetPath(fileId, fileName, request.Folder, request.File.FileName);
            var userId = _currentUserId.GetUserId();

            int? tenantId = null;
            var subdomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            if (!string.IsNullOrEmpty(subdomain))
            {
                var id = await _tenantRepository.GetTenantIdAsync(subdomain, cancellationToken);
                tenantId = id > 0 ? id : null;
            }

            var cdnUrl = await _fileService.UploadFileAsync(request.File, path);
            if (cdnUrl == null)
                return FileErrors.UploadFailed;

            var newFile = new Domain.Entites.File
            {
                Id = fileId,
                Name = fileName,
                Size = request.File.Length,
                Type = fileType,
                Url = cdnUrl,
                UploadedById = userId,
                Status = request.EnableEmbedding ? FileStatus.Processing : FileStatus.Success,
                TenantId = tenantId
            };
            await _fileRepository.CreateAsync(newFile, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            if (request.EnableEmbedding)
                await _fileService.CallAIService(request.File, fileType, fileId);

            return new UploadFileDto
            {
                FileId = fileId,
                FileType = fileType.ToString(),
                Url = cdnUrl,
                OriginalName = fileName,
                Size = request.File.Length
            };
        }
    }
}
