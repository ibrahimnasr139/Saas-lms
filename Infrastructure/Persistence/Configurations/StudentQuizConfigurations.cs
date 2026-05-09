using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentQuizConfigurations : IEntityTypeConfiguration<StudentQuiz>
    {
        public void Configure(EntityTypeBuilder<StudentQuiz> builder)
        {
            builder.HasOne(sq => sq.Student)
                .WithMany(s => s.StudentQuizzes)
                .HasForeignKey(sq => sq.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sq => sq.Subject)
                .WithMany(ss => ss.StudentQuizzes)
                .HasForeignKey(sq => sq.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sq => sq.Chapter)
                .WithMany(sc => sc.StudentQuizzes)
                .HasForeignKey(sq => sq.ChapterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}