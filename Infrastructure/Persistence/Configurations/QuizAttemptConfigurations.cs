using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class QuizAttemptConfigurations : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {
            builder.HasIndex(x => new { x.StudentId, x.ModuleItemId })
                   .IsUnique();

            builder.HasOne(qa => qa.Quiz)
                    .WithMany(q => q.Attempts)
                    .HasForeignKey(qa => qa.ModuleItemId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(qa => qa.Student)
                   .WithMany(s => s.QuizAttempts)
                   .HasForeignKey(qa => qa.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
