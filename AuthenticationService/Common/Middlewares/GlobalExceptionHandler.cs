using AuthenticationService.Common.Exceptions;
using AuthenticationService.Common.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace AuthenticationService.Common.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            var errors = new List<string> { "INTERNAL_SERVER_ERROR" };

            if (exception is ValidationException validationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Validation failed.";
                
                errors = validationException.Errors
                    .Select(e => !string.IsNullOrEmpty(e.ErrorCode) ? e.ErrorCode : "VAL_REQUIRED_FIELD")
                    .Distinct()
                    .ToList();
            }
            else if (exception is AppException appException)
            {
                statusCode = appException.StatusCode;
                message = appException.Message;
                errors = new List<string> { appException.ErrorCode };
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)statusCode;

            var responseEnvelope = ApiResponse<object>.Failure(errors, message, (int)statusCode);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await httpContext.Response.WriteAsJsonAsync(responseEnvelope, jsonOptions, cancellationToken);

            return true;
        }
    }
}
