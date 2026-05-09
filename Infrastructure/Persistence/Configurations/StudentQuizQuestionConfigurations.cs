using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentQuizQuestionConfigurations : IEntityTypeConfiguration<StudentQuizQuestion>
    {
        public void Configure(EntityTypeBuilder<StudentQuizQuestion> builder)
        {
            builder.Property(l => l.Options)
                .HasColumnType("jsonb")
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<List<StudentQuizQuestionOption>>(v, (JsonSerializerOptions?)null)!
               );

            builder.HasOne(sqq => sqq.StudentQuiz)
                .WithMany(sq => sq.StudentQuizQuestions)
                .HasForeignKey(sqq => sqq.StudentQuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}