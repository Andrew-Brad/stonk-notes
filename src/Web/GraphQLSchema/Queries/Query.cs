using StonkNotes.Application.DayNotes.Queries.GetDayNoteById;

namespace StonkNotes.Web.GraphQLSchema;

public class Query
{
    public async Task<DayNote> GetDayNote(int id, [Service] IMediator mediator)
    {
        var queryResult = await mediator.Send(new GetDayNoteByIdQuery(id));

        // todo: Automapper for App query -> GQL Type
        return new DayNote()
        {
            EntryDate = queryResult.EntryDate,
            SummaryText = queryResult.SummaryText,
            NoteText = queryResult.FullNoteText,
            MarketCondition = queryResult.MarketCondition,
            Mood = queryResult.Mood,
            MarketVolatility = queryResult.MarketVolatility
        };
    }
}
