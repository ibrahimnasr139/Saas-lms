using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class FlashCardConfigurations : IEntityTypeConfiguration<FlashCard>
    {
        public void Configure(EntityTypeBuilder<FlashCard> builder)
        {
            builder.HasOne(fc => fc.FlashCardDeck)
                .WithMany(fd => fd.FlashCards)
                .HasForeignKey(fc => fc.DeckId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fc => fc.Student)
                .WithMany(s => s.FlashCards)
                .HasForeignKey(fc => fc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(fc => fc.FlashCardReviews)
                .WithOne(fr => fr.FlashCard)
                .HasForeignKey(fr => fr.FlashCardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}