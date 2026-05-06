using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentSubjectConfigurations : IEntityTypeConfiguration<StudentSubject>
    {
        public void Configure(EntityTypeBuilder<StudentSubject> builder)
        {
            builder.HasIndex(x => new { x.StudentId, x.AvailableSubjectId })
               .IsUnique();

            builder.HasOne(x => x.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AvailableSubject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(x => x.AvailableSubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}