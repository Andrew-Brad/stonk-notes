using FluentAssertions;
using NUnit.Framework;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Domain.UnitTests.ValueObjects;

public class ShortTextTests
{
    private readonly string _validString = Guid.NewGuid().ToString();

    [Test]
    public void From_ValidString_ShouldReturnValueObject()
    {
        var shortText = ShortText.From(_validString);
        shortText.TextBody.Should().Be(_validString);
    }

    [Test]
    public void ToString_ShouldReturnValue()
    {
        var shortText = ShortText.From(_validString);
        shortText.ToString().Should().Be(_validString);
    }

    [Test]
    public void From_StringExactMaxLength_ShouldReturnValueObject()
    {
        var input = new string('a', 128);
        var shortText = ShortText.From(input);
        shortText.TextBody.Should().Be(input);
    }

    [Test]
    public void From_StringTooLong_ShouldThrow()
    {
        var input = new string('a', 1025);
        FluentActions.Invoking(() => ShortText.From(input)).Should().Throw<ArgumentException>();
    }
}
