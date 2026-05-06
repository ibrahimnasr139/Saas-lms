using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasIndex(e => new { e.StudentId, e.CourseId, e.TenantId })
                    .IsUnique();

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Tenant)
                   .WithMany(t => t.Enrollments)
                   .HasForeignKey(e => e.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Order)
                   .WithMany(o => o.Enrollments)
                   .HasForeignKey(e => e.OrderId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.TenantMember)
                   .WithMany(tm => tm.Enrollments)
                   .HasForeignKey(e => e.InvitedBy)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Module)
                   .WithMany(m => m.Enrollments)
                   .HasForeignKey(e => e.CurrentModuleId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.ModuleItem)
                   .WithMany(mi => mi.Enrollments)
                   .HasForeignKey(e => e.CurrentModuleItemId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}