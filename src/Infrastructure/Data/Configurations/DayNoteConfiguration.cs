using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StonkNotes.Domain.Entities;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Infrastructure.Data.Configurations;

public class DayNoteConfiguration : IEntityTypeConfiguration<DayNote>
{
    public void Configure(EntityTypeBuilder<DayNote> builder)
    {
        builder.Property(t => t.EntryDate)
            .IsRequired();

        builder.Property(t => t.SummaryText)
            .HasMaxLength(ShortText.MaxLength)
            .IsRequired()
            .HasConversion(
                v => v.TextBody,
                v => ShortText.From(v));

        builder.Property(t => t.FullNoteText)
            .HasMaxLength(LongText.MaxLength)
            .IsRequired(false)
            .HasConversion(
                v => v.HasValue ? v.Value.TextBody : null,
                v => LongText.FromOrNull(v));

        builder.Property(t => t.MarketCondition)
            .IsRequired();

        builder.Property(t => t.Mood)
            .IsRequired();

        builder.Property(t => t.MarketVolatility)
            .IsRequired();
    }
}
