using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Application.Common.ValidationExtensions;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.DayNotes.Commands.UpdateDayNote;

public class UpdateDayNoteCommandValidator : AbstractValidator<UpdateDayNoteCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDayNoteCommandValidator(IApplicationDbContext context)
    {
        // todo: copy-paste from create
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

        RuleFor(command => command.MarketCondition).MustNotContainUnknownEnumValue();
        RuleFor(command => command.MarketVolatility).MustNotContainUnknownEnumValue();
        RuleFor(command => command.Mood).MustNotContainUnknownEnumValue();
    }
}
