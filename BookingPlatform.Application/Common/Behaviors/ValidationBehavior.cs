using BookingPlatform.Application.Common;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var description = string.Join("; ", failures.Select(f => f.ErrorMessage));
            var error = new Error("ValidationError", ErrorType.Validation, description, 400);

            var resultType = typeof(TResponse);
            var genericArg = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(genericArg)
                .GetMethod("op_Implicit", new[] { typeof(Error) })!;

            return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
        }

        return await next();
    }
}