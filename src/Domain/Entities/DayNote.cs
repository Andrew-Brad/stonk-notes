using Ardalis.GuardClauses;

namespace StonkNotes.Domain.Entities;

public class DayNote(
    DateOnly entryDate,
    ShortText summaryText,
    LongText? fullNoteText,
    MarketCondition marketCondition,
    Mood mood,
    MarketVolatility marketVolatility) : BaseEntity
{
    public DateOnly EntryDate { get; private set; } = entryDate;
    public ShortText SummaryText { get; private set; } = summaryText;
    public LongText? FullNoteText { get; private set; } = fullNoteText;
    public MarketCondition MarketCondition { get; private set; } = marketCondition;
    public Mood Mood { get; private set; } = mood;
    public MarketVolatility MarketVolatility { get; private set; } = marketVolatility;

    public void UpdateDayNote(DateOnly entryDate,
        ShortText summaryText,
        LongText? fullNoteText,
        MarketCondition marketCondition,
        Mood mood,
        MarketVolatility marketVolatility)
    {
        EntryDate = entryDate;
        SummaryText = summaryText;
        FullNoteText = fullNoteText;
        MarketCondition = Guard.Against.Default(marketCondition);
        Mood = Guard.Against.Default(mood);
        MarketVolatility = Guard.Against.Default(marketVolatility);
    }
}
