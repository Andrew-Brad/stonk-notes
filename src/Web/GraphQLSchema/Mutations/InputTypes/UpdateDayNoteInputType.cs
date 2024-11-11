using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Web.GraphQLSchema;

public record UpdateDayNoteInput(
    int Id,
    DateOnly EntryDate,
    string SummaryText,
    string NoteText,
    MarketCondition MarketCondition,
    Mood Mood,
    MarketVolatility MarketVolatility);

public record UpdateDayNotePayload(int Id); //, DateOnly EntryDate, string NoteText);

public class UpdateDayNoteInputType : InputObjectType<UpdateDayNoteInput>
{
    protected override void Configure(
        IInputObjectTypeDescriptor<UpdateDayNoteInput> descriptor)
    {
        // Omitted code for brevity
    }
}
