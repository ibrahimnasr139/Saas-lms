using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class AiAssistantMessageConfigurations : IEntityTypeConfiguration<AiAssistantMessage>
    {
        public void Configure(EntityTypeBuilder<AiAssistantMessage> builder)
        {
            builder.Property(x => x.Content)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.HasOne(ai => ai.Student)
                .WithMany(st => st.AiAssistantMessages)
                .HasForeignKey(ai => ai.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ai => ai.ModuleItem)
                .WithMany(mi => mi.AiAssistantMessages)
                .HasForeignKey(ai => ai.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}