using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class MetadataConfigurations : IEntityTypeConfiguration<Metadata>
    {
        public void Configure(EntityTypeBuilder<Metadata> builder)
        {
            builder.HasOne(m => m.File)
                .WithMany(t => t.Metadatas)
                .HasForeignKey(m => m.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}