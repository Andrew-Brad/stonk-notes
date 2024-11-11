using Microsoft.Extensions.Logging;
using ValidationException = StonkNotes.Application.Common.Exceptions.ValidationException;

namespace StonkNotes.Application.Common.Behaviours;

public class UnhandledExceptionLoggingBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogError(ex, "StonkNotes Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
            throw;
        }
    }
}
