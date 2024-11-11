using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Web.GraphQLSchema;

public record DayNote
{
    public DateOnly EntryDate { get; init; }

    public required string SummaryText { get; init; }

    public required string NoteText { get; init; }

    public MarketCondition MarketCondition { get; set; }

    public Mood Mood { get; set; }

    public MarketVolatility MarketVolatility { get; set; }
}

public class DayNoteType : ObjectType<DayNote>
{
    protected override void Configure(IObjectTypeDescriptor<DayNote> descriptor)
    {
        descriptor.Field(f => f.EntryDate); // Type inferred from DayNote
        descriptor.Field(f => f.SummaryText); // Type inferred from DayNote
        descriptor.Field(f => f.NoteText); // Type inferred from DayNote
    }
}
