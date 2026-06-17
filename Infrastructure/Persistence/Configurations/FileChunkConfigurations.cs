using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class FileChunkConfigurations : IEntityTypeConfiguration<FileChunk>
    {
        public void Configure(EntityTypeBuilder<FileChunk> builder)
        {
            var converter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!)!
            );

            builder.Property(f => f.Metadata)
                .HasConversion(converter!)
                .HasColumnType("jsonb");

            builder.HasOne(fc => fc.File)
                .WithMany(f => f.FileChunks)
                .HasForeignKey(fc => fc.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fc => fc.Tenant)
                .WithMany(t => t.FileChunks)
                .HasForeignKey(fc => fc.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(fc => fc.FileId);

            builder.HasIndex(fc => new { fc.FileId, fc.ChunkIndex })
                .IsUnique();
        }
    }
}