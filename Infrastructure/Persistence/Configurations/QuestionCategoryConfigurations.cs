using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class QuestionCategoryConfigurations : IEntityTypeConfiguration<QuestionCategory>
    {
        public void Configure(EntityTypeBuilder<QuestionCategory> builder)
        {
            builder.HasIndex(qc => new { qc.Title, qc.TenantId }).IsUnique();
        }
    }
}
