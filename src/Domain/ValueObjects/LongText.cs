using Ardalis.GuardClauses;

namespace StonkNotes.Domain.ValueObjects;

public readonly record struct LongText
{
    public const Int16 MaxLength = 4096;
    public string TextBody { get; private init; }

    // From() is a Factory Method that intentionally fails fast for any bad inputs.
    // User input should be validated and gracefully return errors elsewhere in the pipeline
    // Method reference: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Domain/ValueObjects/Colour.cs#L5
    public static LongText From(string text)
    {
        Guard.Against.LengthOutOfRange(text, 1, MaxLength, nameof(text));
        return new LongText { TextBody = text };
    }

    public static LongText? FromOrNull(string? text) // consider Maybe types in Result framework
    {
        if (string.IsNullOrEmpty(text)) return null;
        Guard.Against.LengthOutOfRange(text, 1, MaxLength, nameof(text));
        return new LongText { TextBody = text };
    }

    public override string ToString() => TextBody;

    //public static implicit operator string(ShortText st) => st.TextBody;
    //public static explicit operator Digit(byte b) => new Digit(b);
}
