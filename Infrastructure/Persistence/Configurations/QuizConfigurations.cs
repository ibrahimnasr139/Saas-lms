using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class QuizConfigurations : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(x => x.ModuleItemId);
            builder.HasOne(l => l.ModuleItem)
                    .WithOne(mi => mi.Quiz)
                    .HasForeignKey<Quiz>(l => l.ModuleItemId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}