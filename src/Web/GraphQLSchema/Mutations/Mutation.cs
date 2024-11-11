using StonkNotes.Application.DayNotes.Commands.CreateDayNote;
using StonkNotes.Application.DayNotes.Commands.UpdateDayNote;

namespace StonkNotes.Web.GraphQLSchema;

public class Mutation
{
    public async Task<AddDayNotePayload> CreateDayNote(
        AddDayNoteInput dayNoteInput,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateDayNoteCommand(
            dayNoteInput.EntryDate,
            dayNoteInput.SummaryText,
            dayNoteInput.NoteText,
            dayNoteInput.MarketCondition,
            dayNoteInput.Mood,
            dayNoteInput.MarketVolatility),
            cancellationToken);
        return new AddDayNotePayload(result); //, EntryDate = result.EntryDate, NoteText = result.NoteText, };
    }

    public async Task<UpdateDayNotePayload> UpdateDayNote(
        UpdateDayNoteInput dayNoteInput,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateDayNoteCommand(
                dayNoteInput.Id,
                dayNoteInput.EntryDate,
                dayNoteInput.SummaryText,
                dayNoteInput.NoteText,
                dayNoteInput.MarketCondition,
                dayNoteInput.Mood,
                dayNoteInput.MarketVolatility),
            cancellationToken);
        return new UpdateDayNotePayload(result); //, EntryDate = result.EntryDate, NoteText = result.NoteText, };
    }
}
