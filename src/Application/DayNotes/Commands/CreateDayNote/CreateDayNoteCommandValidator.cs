using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Application.Common.ValidationExtensions;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.DayNotes.Commands.CreateDayNote;

public class CreateDayNoteCommandValidator : AbstractValidator<CreateDayNoteCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDayNoteCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        // RuleFor(v => v.Title)
        //     .NotEmpty()
        //     .MaximumLength(200)
        //     .MustAsync(BeUniqueTitle)
        //     .WithMessage("'{PropertyName}' must be unique.")
        //     .WithErrorCode("Unique");

        // Todo: an argument can be made that this validation expression is duplicative
        // i.e. ShortText.cs should hold these rules and FluentValidation should respect those
        // Perhaps this touches on the idea for a well known error catalog?
        RuleFor(v => v.SummaryText)
            .NotEmpty()
            .MaximumLength(ShortText.MaxLength)
            //.MustAsync(BeUniqueTitle)
            .WithMessage("Text summaries must be 1,024 characters or less.")
            .WithErrorCode("ERR1_TEXT_SUMMARY_LENGTH");

        RuleFor(v => v.FullNoteText)
            .NotEmpty()
            .MaximumLength(LongText.MaxLength)
            //.MustAsync(BeUniqueTitle)
            .WithMessage("Note text must be 4,096 characters or less.")
            .WithErrorCode("ERR2_LONG_TEXT_SUMMARY_LENGTH");

        RuleFor(command => command.MarketCondition).MustNotContainUnknownEnumValue();
        RuleFor(command => command.MarketVolatility).MustNotContainUnknownEnumValue();
        RuleFor(command => command.Mood).MustNotContainUnknownEnumValue();
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return await _context.TodoLists
            .AllAsync(l => l.Title != title, cancellationToken);
    }
}
