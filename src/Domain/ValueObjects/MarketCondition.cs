using Ardalis.GuardClauses;

namespace StonkNotes.Domain.ValueObjects;

public enum MarketCondition : Int16
{
    Unknown = 0,
    Bullish,
    Bearish,
    Neutral,
}
