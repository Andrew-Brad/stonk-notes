namespace StonkNotes.Application.Common.ValidationExtensions;

public static class EnumValidationExtensions
{
    public static IRuleBuilderOptions<T, TEnum> MustNotContainUnknownEnumValue<T, TEnum>(
        this IRuleBuilder<T, TEnum> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .NotEmpty() // todo: Roslyn analyzer rule to enforce all enums have an unknown value defined at index 0
            .WithMessage("The value '{PropertyValue}' is not supported for new usages of '{PropertyName}'.")
            .WithErrorCode("Unknown Enum value is not supported."); // todo: error code catalog?
    }
}
