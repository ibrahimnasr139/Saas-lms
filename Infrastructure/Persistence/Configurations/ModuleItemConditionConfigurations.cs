using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class ModuleItemConditionConfigurations : IEntityTypeConfiguration<ModuleItemCondition>
    {
        public void Configure(EntityTypeBuilder<ModuleItemCondition> builder)
        {
            builder.HasOne(e => e.ModuleItem)
                .WithMany(m => m.Conditions)
                .HasForeignKey(e => e.ModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.RequiredModuleItem)
                .WithMany()
                .HasForeignKey(e => e.RequiredModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
