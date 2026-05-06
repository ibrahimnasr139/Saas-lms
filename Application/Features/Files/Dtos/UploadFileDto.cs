namespace Application.Features.Files.Dtos
{
    public sealed class UploadFileDto
    {
        public string FileId { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string? Url { get; init; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
