using Application.Constants;
using Application.Contracts.Files;
using Application.Features.Files.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Infrastructure.Services
{
    internal class FileService : IFileService
    {
        private readonly IOptions<Common.Options.FileOptions> _fileOptions;
        private readonly IOptions<BunnyOptions> _bunnyOptions;
        private readonly IOptions<AiTranscriptionOptions> _aiOptions;
        private readonly HttpClient _httpClient;
        public FileService(IOptions<Common.Options.FileOptions> fileOptions, IOptions<BunnyOptions> bunnyOptions,
            IOptions<AiTranscriptionOptions> aiOptions, HttpClient httpClient)
        {
            _fileOptions = fileOptions;
            _bunnyOptions = bunnyOptions;
            _httpClient = httpClient;
            _aiOptions = aiOptions;
        }

        public long GetMaxSize(FileType fileType) => fileType switch
        {
            FileType.Video => _fileOptions.Value.VideoMaxSize,
            FileType.Image => _fileOptions.Value.ImageMaxSize,
            _ => _fileOptions.Value.DocumentMaxSize
        };
        public string GetPath(string fileId, string name, string? folder, string originalFileName)
        {
            var sanitizedFolder = string.IsNullOrWhiteSpace(folder) ? string.Empty : folder.Trim('/');
            var sanitizedFileName = name.Replace(" ", "_");
            var extension = Path.GetExtension(originalFileName);

            return string.IsNullOrEmpty(sanitizedFolder)
                ? $"{fileId}_{sanitizedFileName}{extension}"
                : $"{sanitizedFolder}/{fileId}_{sanitizedFileName}{extension}";
        }
        public FileType GetFileType(string contentType) => contentType switch
        {
            var ct when ct.StartsWith(FileConstants.Image) => FileType.Image,
            var ct when ct.StartsWith(FileConstants.Video) => FileType.Video,
            _ => FileType.Document
        };
        public string CreateCdnUrl(string path)
        {
            var encodedPath = HttpUtility.UrlPathEncode(path);
            return $"{_bunnyOptions.Value.CdnUrl.TrimEnd('/')}/{encodedPath.TrimStart('/')}";
        }
        public string CreateUploadUrl(string path)
        {
            var encodedPath = HttpUtility.UrlPathEncode(path);
            return $"https://{_bunnyOptions.Value.HostName}/{_bunnyOptions.Value.StorageZoneName}/{encodedPath.TrimStart('/')}";
        }
        public async Task<string?> UploadFileAsync(IFormFile file, string path)
        {
            var uploadUrl = CreateUploadUrl(path);

            using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            using var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl) { Content = content };
            request.Headers.Add(FileConstants.AccessKey, _bunnyOptions.Value.AccessKey);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode ? CreateCdnUrl(path) : null;
        }
        public async Task CallAIService(IFormFile file, FileType fileType, string fileId)
        {
            using var content = new MultipartFormDataContent();

            await using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, FileConstants.AiRequestFile, file.FileName);
            content.Add(new StringContent(fileId), FileConstants.AiRequestFileId);
            content.Add(new StringContent(fileType.ToString()), FileConstants.AiRequestFileType);
            content.Add(new StringContent(_aiOptions.Value.CallbackUrl), FileConstants.AiRequestCallBackUrl);

            _ = _httpClient.PostAsync(_aiOptions.Value.Url, content);
        }
        public async Task<CreateUploadDto?> CreateUploadCredentialsAsync(string title, long Size, CancellationToken cancellationToken)
        {
            if (Size > _fileOptions.Value.VideoMaxSize)
                return null;

            var libraryId = _bunnyOptions.Value.VideoLibraryId;
            var apiKey = _bunnyOptions.Value.StreamKey;

            var video = await CreateVideoObjectAsync(title, libraryId, apiKey, cancellationToken);
            if (video == null)
                return null;

            var expirationTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
            var signature = GenerateSignature(libraryId, apiKey, expirationTime, video.Guid);

            return new CreateUploadDto
            {
                VideoId = video.Guid,
                LibraryId = libraryId,
                ExpirationTime = expirationTime,
                Signature = signature,
                EmbedUrl = $"{FileConstants.BunnyEmbedBaseUrl}/{libraryId}/{video.Guid}"
            };
        }
        private async Task<BunnyVideoResponseDto?> CreateVideoObjectAsync(string title, string libraryId, string apiKey, CancellationToken cancellationToken)
        {
            var url = $"{FileConstants.BunnyStreamBaseUrl}/{libraryId}/{FileConstants.Videos}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(new { title })
            };
            request.Headers.Add(FileConstants.AccessKey, apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<BunnyVideoResponseDto>(json);
        }
        private static string GenerateSignature(string libraryId, string apiKey, long expirationTime, string videoId)
        {
            var signatureString = $"{libraryId}{apiKey}{expirationTime}{videoId}";
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(signatureString));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
