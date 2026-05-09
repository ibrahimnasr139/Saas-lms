using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentQuizAttemptConfigurations : IEntityTypeConfiguration<StudentQuizAttempt>
    {
        public void Configure(EntityTypeBuilder<StudentQuizAttempt> builder)
        {
            builder.HasOne(sqa => sqa.Student)
                .WithMany(s => s.StudentQuizAttempts)
                .HasForeignKey(sqa => sqa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sqa => sqa.StudentQuiz)
                .WithMany(sq => sq.StudentQuizAttempts)
                .HasForeignKey(sqa => sqa.StudentQuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}