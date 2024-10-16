using FluentValidation;
using FluentValidation.Results;
using MediatR.MinimalApi.Exceptions;

namespace MediatR.MinimalApi.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (validationFailures.Any())
        {
            throw new HttpResponseException(400, "", new Dictionary<string, object?>() { { "errors", validationFailures.Select(x => x.ErrorMessage).ToList() } });
        }

        return await next();
    }

    private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
    {
        IEnumerable<string> values = errors.Select((ValidationFailure x) => $"{Environment.NewLine} -- {x.PropertyName}: {x.ErrorMessage} Severity: {x.Severity.ToString()}");
        return "Validation failed: " + string.Join(string.Empty, values);
    }
}
