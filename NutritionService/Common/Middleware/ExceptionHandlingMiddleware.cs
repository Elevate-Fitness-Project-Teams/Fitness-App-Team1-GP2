using NutritionService.Common.Exceptions;
using NutritionService.Common.Models;
using System.Text.Json;

namespace NutritionService.Common.Middleware;

/// <summary>
/// Central place that turns AppException (and unknowns) into the ApiResponse envelope
/// with the correct HTTP status code and error code expected by the API contract.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled application exception {ErrorCode}", ex.ErrorCode);
            await WriteResponseAsync(context, ex.StatusCode, ex.ErrorCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "SRV_INTERNAL_ERROR", "An unexpected error occurred.");
        }
    }

    private static Task WriteResponseAsync(HttpContext context, int statusCode, string errorCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var payload = ApiResponse<object>.Fail(errorCode, message);
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
