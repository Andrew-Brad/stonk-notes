using FluentValidation.Results;
using ValidationException = StonkNotes.Application.Common.Exceptions.ValidationException;

namespace StonkNotes.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            ValidationResult[] validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, ct)));

            var failures = validationResults
                .Where(validationResult => validationResult.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToArray();

            if (failures.Any()) throw new ValidationException(failures);
        }

        return await next();
    }
}
