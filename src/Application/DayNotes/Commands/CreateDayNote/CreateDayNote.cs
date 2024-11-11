using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Domain.Entities;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.DayNotes.Commands.CreateDayNote;

public record CreateDayNoteCommand(
    DateOnly DayNoteEntryDate,
    string SummaryText,
    string FullNoteText,
    MarketCondition MarketCondition,
    Mood Mood,
    MarketVolatility MarketVolatility
    ) : IRequest<int>;

public class CreateDayNoteCommandHandler : IRequestHandler<CreateDayNoteCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateDayNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateDayNoteCommand request, CancellationToken cancellationToken)
    {
        var summaryText = ShortText.From(request.SummaryText);
        var fullNoteText = LongText.From(request.FullNoteText);
        var entity = new DayNote(request.DayNoteEntryDate, summaryText, fullNoteText, request.MarketCondition, request.Mood, request.MarketVolatility);

        _context.DayNotes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
