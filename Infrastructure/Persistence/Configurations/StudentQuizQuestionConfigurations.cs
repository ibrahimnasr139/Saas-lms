using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentQuizQuestionConfigurations : IEntityTypeConfiguration<StudentQuizQuestion>
    {
        public void Configure(EntityTypeBuilder<StudentQuizQuestion> builder)
        {
            builder.HasOne(sqq => sqq.StudentQuiz)
                .WithMany(sq => sq.StudentQuizQuestions)
                .HasForeignKey(sqq => sqq.StudentQuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}