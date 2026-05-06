using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class LessonViewConfigurations : IEntityTypeConfiguration<LessonView>
    {
        public void Configure(EntityTypeBuilder<LessonView> builder)
        {
            builder.HasOne(lv => lv.Student)
                .WithOne(s => s.LessonView)
                .HasForeignKey<LessonView>(lv => lv.StudentId);
            builder.HasOne(lv => lv.Lesson)
               .WithMany(l => l.LessonViews)
               .HasForeignKey(lv => lv.ModuleItemId);
        }
    }
}
