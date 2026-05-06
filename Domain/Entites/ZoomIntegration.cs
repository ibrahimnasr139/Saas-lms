using Domain.Abstractions;

namespace Domain.Entites
{
    public sealed class ZoomIntegration : IAuditable
    {
        public int Id { get; set; }
        public string ZoomUserId { get; set; } = string.Empty;
        public string ZoomAccountId { get; set; } = string.Empty;
        public string ZoomEmail { get; set; } = string.Empty;
        public string ZoomDisplayName { get; set; } = string.Empty;
        public string ZoomAccountType { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime TokenExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastSyncAt { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<LiveSession> LiveSessions { get; set; } = [];
    }
}
