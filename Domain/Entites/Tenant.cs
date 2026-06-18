using Domain.Abstractions;

namespace Domain.Entites
{
    public sealed class Tenant : IAuditable
    {
        public int Id { get; set; }
        public string PlatformName { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public string? Logo { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Grade> Grades { get; set; } = [];
        public ICollection<TenantRole> TenantRoles { get; set; } = [];
        public ICollection<TenantMember> TenantMembers { get; set; } = [];
        public ICollection<TeachingLevel> TeachingLevels { get; set; } = [];
        public ICollection<Subject> Subjects { get; set; } = [];
        public ICollection<Subscription> Subscriptions { get; set; } = [];
        public ICollection<Course> Courses { get; set; } = [];
        public ICollection<TenantUsage> TenantUsages { get; set; } = [];
        public ICollection<TenantInvite> TenantInvites { get; set; } = [];
        public ICollection<LiveSession> LiveSessions { get; set; } = [];
        public ICollection<ZoomIntegration> ZoomIntegrations { get; set; } = [];
        public ICollection<ZoomOAuthState> ZoomOAuthStates { get; set; } = [];
        public ICollection<TenantPage> TenantPages { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
        public ICollection<Question> Questions { get; set; } = [];
        public WebsiteSetting WebsiteSetting { get; set; } = null!;
        public WebsiteAppearanceSetting WebsiteAppearnceSetting { get; set; } = null!;
        public EmailSetting EmailSetting { get; set; } = null!;
        public NotificationSetting NotificationSetting { get; set; } = null!;
        public ICollection<StudentGrade> StudentGrades { get; set; } = [];
        public ICollection<CourseInvite> CourseInvites { get; set; } = [];
        public ICollection<DicussionThread> DicussionThreads { get; set; } = [];
        public ICollection<DicussionThreadRead> DicussionReads { get; set; } = [];
        public ICollection<DicussionThreadReply> DicussionReplies { get; set; } = [];
        public ICollection<Announcement> Announcements { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; } = [];
        public ICollection<FileChunk> FileChunks { get; set; } = [];
        public ICollection<TenantPageVisit> TenantPageVisits { get; set; } = [];
    }
}