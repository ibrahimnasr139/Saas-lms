using Domain.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        internal DbSet<RefreshToken> RefreshTokens { get; set; }
        internal DbSet<Plan> Plans { get; set; }
        internal DbSet<PlanPricing> PlanPricings { get; set; }
        internal DbSet<PlanFeature> PlanFeatures { get; set; }
        internal DbSet<Feature> Features { get; set; }
        internal DbSet<Tenant> Tenants { get; set; }
        internal DbSet<TenantRole> TenantRoles { get; set; }
        internal DbSet<TenantMember> TenantMembers { get; set; }
        internal DbSet<Grade> Grades { get; set; }
        internal DbSet<TeachingLevel> TeachingLevels { get; set; }
        internal DbSet<Subject> Subjects { get; set; }
        internal DbSet<Permission> Permissions { get; set; }
        internal DbSet<Domain.Entites.File> Files { get; set; }
        internal DbSet<RolePermission> RolePermissions { get; set; }
        internal DbSet<Subscription> Subscriptions { get; set; }
        internal DbSet<Course> Courses { get; set; }
        internal DbSet<Student> Students { get; set; }
        internal DbSet<Enrollment> Enrollments { get; set; }
        internal DbSet<CourseProgress> CourseProgresses { get; set; }
        internal DbSet<TenantUsage> TenantUsage { get; set; }
        internal DbSet<TenantInvite> TenantInvites { get; set; }
        internal DbSet<Module> Modules { get; set; }
        internal DbSet<ModuleItem> ModuleItems { get; set; }
        internal DbSet<Quiz> Quizzes { get; set; }
        internal DbSet<Lesson> Lessons { get; set; }
        internal DbSet<Assignment> Assignments { get; set; }
        internal DbSet<ModuleItemCondition> ModuleItemConditions { get; set; }
        internal DbSet<LiveSession> LiveSessions { get; set; }
        internal DbSet<ZoomIntegration> ZoomIntegrations { get; set; }
        internal DbSet<ZoomOAuthState> ZoomOAuthStates { get; set; }
        internal DbSet<SessionParticipant> SessionParticipants { get; set; }
        internal DbSet<TenantPage> TenantPages { get; set; }
        internal DbSet<PageBlock> PageBlocks { get; set; }
        internal DbSet<BlockType> BlockTypes { get; set; }
        internal DbSet<Order> Orders { get; set; }
        internal DbSet<OrderTimeLine> OrderTimeLines { get; set; }
        internal DbSet<PaymentMethod> PaymentMethods { get; set; }
        internal DbSet<Question> Questions { get; set; }
        internal DbSet<QuestionCategory> Categories { get; set; }
        internal DbSet<QuizQuestion> QuizQuestions { get; set; }
        internal DbSet<QuizAttempt> QuizAttempts { get; set; }
        internal DbSet<LessonView> LessonViews { get; set; }
        internal DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        internal DbSet<WebsiteSetting> WebsiteSettings { get; set; }
        internal DbSet<WebsiteAppearanceSetting> WebsiteAppearanceSettings { get; set; }
        internal DbSet<EmailSetting> EmailSettings { get; set; }
        internal DbSet<NotificationSetting> NotificationSettings { get; set; }
        internal DbSet<Answer> Answers { get; set; }
        internal DbSet<StudentGrade> StudentGrades { get; set; }
        internal DbSet<CourseInvite> CourseInvites { get; set; }
        internal DbSet<DicussionThread> DicussionThreads { get; set; }
        internal DbSet<DicussionThreadReply> DicussionThreadReplies { get; set; }
        internal DbSet<DicussionThreadRead> DicussionThreadReads { get; set; }
        internal DbSet<Announcement> Announcements { get; set; }
        internal DbSet<Schedule> Schedules { get; set; }
        internal DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        internal DbSet<AvailableSubject> AvailableSubjects { get; set; }
        internal DbSet<StudentSubject> StudentSubjects { get; set; }
        internal DbSet<StudentStreak> StudentStreaks { get; set; }
        internal DbSet<Friend> Friends { get; set; }
        internal DbSet<LessonVideoSegmant> LessonVideoSegmants { get; set; }
        internal DbSet<Level> Levels { get; set; }
        internal DbSet<VideoTimestamp> VideoTimestamps { get; set; }
        internal DbSet<Transcript> Transcripts { get; set; }
        internal DbSet<Metadata> Metadata { get; set; }
        internal DbSet<AiAssistantMessage> AiAssistantMessages { get; set; }
        internal DbSet<StudentChapter> StudentChapters { get; set; }
        internal DbSet<FlashCard> FlashCards { get; set; }
        internal DbSet<FlashCardDeck> FlashCardDecks { get; set; }
        internal DbSet<FlashCardReview> FlashCardReviews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        override public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && e.State == EntityState.Modified);
            foreach (var entityEntry in entries)
            {
                ((IAuditable)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}