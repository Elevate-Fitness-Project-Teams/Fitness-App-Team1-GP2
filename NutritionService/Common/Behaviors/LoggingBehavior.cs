using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NutritionService.Common.Behaviors;

/// <summary>Logs every request/response pair with timing, useful for tracing vertical slices end to end.</summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling {Request}", requestName);

        var response = await next();

        stopwatch.Stop();
        _logger.LogInformation("Handled {Request} in {Elapsed}ms", requestName, stopwatch.ElapsedMilliseconds);
        return response;
    }
}
