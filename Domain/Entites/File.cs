using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entites
{
    public sealed class File : IAuditable
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public FileType Type { get; set; }
        public FileStatus Status { get; set; }
        public string Url { get; set; } = string.Empty;
        public string StorageProvider { get; set; } = "Bunny";
        public Dictionary<string, string>? Metadata { get; set; }
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public string UploadedById { get; set; } = string.Empty;
        public ApplicationUser UploadedBy { get; set; } = null!;
        public AssignmentSubmission? AssignmentSubmission { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<VideoTimestamp> videoTimestamps { get; set; } = [];
        public Transcript Transcript { get; set; } = null!;
    }
}