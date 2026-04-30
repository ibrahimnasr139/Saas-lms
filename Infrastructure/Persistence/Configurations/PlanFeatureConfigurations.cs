using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class PlanFeatureConfigurations : IEntityTypeConfiguration<PlanFeature>
    {
        public void Configure(EntityTypeBuilder<PlanFeature> builder)
        {
            builder.HasKey(pf => pf.Id);

            builder.Property(pf => pf.LimitUnit)
                    .HasMaxLength(50)
                    .IsRequired();

            builder.Property(pf => pf.Note)
                   .HasMaxLength(500);

            builder.Property(f => f.Description)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.HasOne(pf => pf.Plan)
                   .WithMany(p => p.PlanFeatures)
                   .HasForeignKey(pf => pf.PlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pf => pf.Feature)
                   .WithMany(f => f.PlanFeatures)
                   .HasForeignKey(pf => pf.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pf => new { pf.PlanId, pf.FeatureId })
                  .IsUnique();
        }
    }
}
