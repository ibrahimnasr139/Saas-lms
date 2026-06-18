using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class TenantPageVisitConfigurations : IEntityTypeConfiguration<TenantPageVisit>
    {
        public void Configure(EntityTypeBuilder<TenantPageVisit> builder)
        {
            builder.HasIndex(x => new { x.TenantId, x.VisitorId, x.PageUrl })
                   .IsUnique();

            builder.HasOne(x => x.Tenant)
                .WithMany(t => t.TenantPageVisits)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}