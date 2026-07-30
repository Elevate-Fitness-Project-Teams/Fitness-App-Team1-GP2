using FluentValidation;
using MediatR;
using NutritionService.Common.Exceptions;

namespace NutritionService.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs every registered FluentValidation validator
/// for the incoming request before the handler executes. Failing validation short
/// circuits the pipeline with VAL_REQUIRED_FIELD (400).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var message = string.Join(" | ", failures.Select(f => f.ErrorMessage));
            throw new ValidationAppException(message);
        }

        return await next();
    }
}
