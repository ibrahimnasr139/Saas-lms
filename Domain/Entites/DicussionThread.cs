using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entites
{
    public sealed class DicussionThread : IAuditable
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int RepliesCount { get; set; } = 0;
        public ModuleItemType ItemType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public int ItemId { get; set; }
        public ModuleItem ModuleItem { get; set; } = null!;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public ICollection<DicussionThreadReply> Replies { get; set; } = [];
        public ICollection<DicussionThreadRead> DicussionReads { get; set; } = [];
    }
}