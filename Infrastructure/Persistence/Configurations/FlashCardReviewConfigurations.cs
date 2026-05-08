using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class FlashCardReviewConfigurations : IEntityTypeConfiguration<FlashCardReview>
    {
        public void Configure(EntityTypeBuilder<FlashCardReview> builder)
        {
            builder.HasOne(fr => fr.FlashCard)
                .WithMany(fc => fc.FlashCardReviews)
                .HasForeignKey(fr => fr.FlashCardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fr => fr.Student)
                .WithMany(s => s.FlashCardReviews)
                .HasForeignKey(fr => fr.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}