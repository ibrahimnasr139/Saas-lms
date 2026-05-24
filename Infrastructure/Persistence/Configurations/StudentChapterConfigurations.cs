using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class StudentChapterConfigurations : IEntityTypeConfiguration<StudentChapter>
    {
        public void Configure(EntityTypeBuilder<StudentChapter> builder)
        {
            var converter = new ValueConverter<Dictionary<string, string>, string>(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                 v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!)!);

            var comparer = new ValueComparer<Dictionary<string, string>?>(
                (d1, d2) =>
                    d1 == d2 ||
                    (d1 != null && d2 != null && d1.SequenceEqual(d2)),

                d => d == null
                    ? 0
                    : d.Aggregate(
                        0,
                        (hash, pair) => HashCode.Combine(hash, pair.Key, pair.Value)),

                d => d == null
                    ? null
                    : d.ToDictionary(pair => pair.Key, pair => pair.Value)
            );

            builder.Property(sc => sc.Metadata)
                .HasConversion(converter!)
                .IsRequired(false)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(comparer);

            builder.HasOne(sc => sc.AvailableSubject)
                .WithMany(s => s.StudentChapters)
                .HasForeignKey(sc => sc.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sc => sc.FlashCardDecks)
                .WithOne(fd => fd.StudentChapter)
                .HasForeignKey(fd => fd.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}