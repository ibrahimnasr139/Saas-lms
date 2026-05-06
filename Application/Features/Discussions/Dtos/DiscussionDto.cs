using Domain.Enums;

namespace Application.Features.Discussions.Dtos
{
    public sealed class DiscussionDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? ItemTitle { get; set; }
        public ModuleItemType? ItemType { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Unread { get; set; }
        public int RepliesCount { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }
}