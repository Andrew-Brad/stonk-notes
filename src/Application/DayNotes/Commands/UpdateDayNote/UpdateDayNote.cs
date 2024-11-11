using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Application.DayNotes.Commands.CreateDayNote;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.DayNotes.Commands.UpdateDayNote;

public record UpdateDayNoteCommand(
    int Id,
    DateOnly DayNoteEntryDate,
    string SummaryText,
    string FullNoteText,
    MarketCondition MarketCondition,
    Mood Mood,
    MarketVolatility MarketVolatility
) : CreateDayNoteCommand(DayNoteEntryDate, SummaryText, FullNoteText, MarketCondition, Mood, MarketVolatility);

public class UpdateDayNoteCommandHandler : IRequestHandler<UpdateDayNoteCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateDayNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateDayNoteCommand request, CancellationToken ct)
    {
        var summaryText = ShortText.From(request.SummaryText);
        var fullNoteText = LongText.From(request.FullNoteText);

        var dayNote = await _context.DayNotes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: ct);
        if (dayNote == null)
        {
            // proper error code and flow back to app code has not been decided yet
            throw new NotSupportedException($"Entity with id {request.Id} was not found.");
        }
        dayNote.UpdateDayNote(request.DayNoteEntryDate, summaryText, fullNoteText, request.MarketCondition, request.Mood, request.MarketVolatility);
        await _context.SaveChangesAsync(ct);
        return dayNote.Id;
    }
}
