using StonkNotes.Domain.Entities;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.DayNotes.Queries.GetDayNoteById;

public record DayNoteDto
{
    public int Id { get; init; }
    public required string SummaryText { get; init; }
    public required string FullNoteText { get; init; }
    public DateOnly EntryDate { get; init; }

    public MarketCondition MarketCondition { get; init; }

    public Mood Mood { get; init; }

    public MarketVolatility MarketVolatility { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DayNote, DayNoteDto>();
        }
    }
}
