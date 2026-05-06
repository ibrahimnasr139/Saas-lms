using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class LessonConfigurations : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(e => e.ModuleItemId);
            builder.Property(l => l.Resources)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Resource>>(v, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!
                );
            builder.HasOne(l => l.ModuleItem)
                .WithOne(mi => mi.Lesson)
                .HasForeignKey<Lesson>(l => l.ModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
