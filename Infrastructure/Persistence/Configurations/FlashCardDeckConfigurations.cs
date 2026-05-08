using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal sealed class FlashCardDeckConfigurations : IEntityTypeConfiguration<FlashCardDeck>
    {
        public void Configure(EntityTypeBuilder<FlashCardDeck> builder)
        {
            builder.HasOne(fd => fd.StudentSubject)
                .WithMany(ss => ss.FlashCardDecks)
                .HasForeignKey(fd => fd.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fd => fd.StudentChapter)
                .WithMany(sc => sc.FlashCardDecks)
                .HasForeignKey(fd => fd.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fd => fd.Student)
                .WithMany(s => s.FlashCardDecks)
                .HasForeignKey(fd => fd.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(fd => fd.FlashCards)
                .WithOne(fc => fc.FlashCardDeck)
                .HasForeignKey(fc => fc.DeckId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}