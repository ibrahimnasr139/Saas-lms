using Application.Features.Files.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Files
{
    public interface IFileService
    {
        long GetMaxSize(FileType fileType);
        string GetPath(string fileId, string name, string? folder, string originalFileName);
        FileType GetFileType(string contentType);
        string CreateCdnUrl(string path);
        string CreateUploadUrl(string path);
        Task<string?> UploadFileAsync(IFormFile file, string path);
        Task CallAIService(IFormFile file, FileType fileType, string fileId);
        Task<CreateUploadDto?> CreateUploadCredentialsAsync(string title, long Size, CancellationToken cancellationToken);
    }
}
