using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class AnswerConfigurations : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(a => new { a.QuizQuestionId, a.AttemptId });

            builder.HasOne(a => a.QuizQuestion)
                .WithMany(qq => qq.Answers)
                .HasForeignKey(a => a.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Attempt)
                   .WithMany(qa => qa.Answers)
                   .HasForeignKey(a => a.AttemptId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
