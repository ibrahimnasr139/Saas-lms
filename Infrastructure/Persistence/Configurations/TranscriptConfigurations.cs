using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class TranscriptConfigurations : IEntityTypeConfiguration<Transcript>
    {
        public void Configure(EntityTypeBuilder<Transcript> builder)
        {
            builder.HasOne(t => t.File)
                .WithOne(f => f.Transcript)
                .HasForeignKey<Transcript>(t => t.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}