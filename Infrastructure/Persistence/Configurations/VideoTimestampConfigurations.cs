using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class VideoTimestampConfigurations : IEntityTypeConfiguration<VideoTimestamp>
    {
        public void Configure(EntityTypeBuilder<VideoTimestamp> builder)
        {
            builder.HasOne(vt => vt.File)
                .WithMany(f => f.videoTimestamps)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}