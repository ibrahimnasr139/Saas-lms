using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                   )
                   .Metadata.SetValueComparer(new ValueComparer<List<Resource>>(
                       (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null)
                                   == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),

                       c => c == null
                           ? 0
                           : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),

                       c => c == null
                           ? new List<Resource>()
                           : JsonSerializer.Deserialize<List<Resource>>(
                               JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                               (JsonSerializerOptions?)null
                           )!
                   ));

            builder.HasOne(l => l.ModuleItem)
                .WithOne(mi => mi.Lesson)
                .HasForeignKey<Lesson>(l => l.ModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.File)
                   .WithOne(f => f.Lesson)
                   .HasForeignKey<Lesson>(l => l.VideoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}