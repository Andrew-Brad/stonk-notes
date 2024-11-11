using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Web.GraphQLSchema;

public record AddDayNoteInput(
    DateOnly EntryDate,
    string SummaryText,
    string NoteText,
    MarketCondition MarketCondition,
    Mood Mood,
    MarketVolatility MarketVolatility);

public record AddDayNotePayload(int Id); //, DateOnly EntryDate, string NoteText);

public class AddDayNoteInputType : InputObjectType<AddDayNoteInput>
{
    protected override void Configure(
        IInputObjectTypeDescriptor<AddDayNoteInput> descriptor)
    {
        // Omitted code for brevity
    }
}
