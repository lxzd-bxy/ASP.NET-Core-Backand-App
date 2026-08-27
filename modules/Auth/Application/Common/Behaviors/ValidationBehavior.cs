using MediatR;
using ErrorOr;
using FluentValidation;

namespace LxzdBxy.Backend.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any())
            return await next(ct);

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                .ToList();

            var responseType = typeof(TResponse);
            if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(ErrorOr<>))
                throw new InvalidOperationException("TResponse must be ErrorOr<T>");

            var successType = responseType.GetGenericArguments()[0];
            var errorOrType = typeof(ErrorOr<>).MakeGenericType(successType);

            var fromMethod = errorOrType.GetMethod(nameof(ErrorOr<>.From), [typeof(List<Error>)]) ??
            throw new InvalidOperationException($"Method 'From' not found on type {errorOrType.Name}");
            var result = fromMethod.Invoke(null, [errors]) ??
            throw new InvalidOperationException("Failed to create ErrorOr result.");
            return (TResponse)result;
        }

        return await next(ct);
    }
}