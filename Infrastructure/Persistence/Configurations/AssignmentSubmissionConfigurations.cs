using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class AssignmentSubmissionConfigurations : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId);

            builder.HasOne(s => s.Student)
                .WithMany(s => s.AssignmentSubmissions)
                .HasForeignKey(s => s.StudentId);

            builder.HasOne(s => s.File)
                .WithOne(f => f.AssignmentSubmission)
                .HasForeignKey<AssignmentSubmission>(s => s.FileId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
