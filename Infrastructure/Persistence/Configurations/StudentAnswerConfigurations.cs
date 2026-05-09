using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentAnswerConfigurations : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.HasOne(sa => sa.StudentQuizQuestion)
                .WithMany(sqq => sqq.StudentAnswers)
                .HasForeignKey(sa => sa.StudentQuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sa => sa.StudentQuizAttempt)
                .WithMany(sqa => sqa.StudentAnswers)
                .HasForeignKey(sa => sa.StudentQuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}