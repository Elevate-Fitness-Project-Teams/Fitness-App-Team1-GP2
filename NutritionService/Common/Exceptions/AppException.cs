namespace NutritionService.Common.Exceptions;

/// <summary>
/// Base for every domain/application exception. ErrorCode maps 1:1 to the codes
/// defined in the backend spec (e.g. RES_MEAL_NOT_FOUND, FCE_METRICS_NOT_CALCULATED).
/// </summary>
public abstract class AppException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    protected AppException(string errorCode, int statusCode, string message) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string errorCode, string message)
        : base(errorCode, StatusCodes.Status404NotFound, message) { }
}

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(string message, string errorCode = "VAL_REQUIRED_FIELD")
        : base(errorCode, StatusCodes.Status400BadRequest, message) { }
}

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string errorCode, string message, int statusCode = StatusCodes.Status400BadRequest)
        : base(errorCode, statusCode, message) { }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string errorCode, string message)
        : base(errorCode, StatusCodes.Status403Forbidden, message) { }
}

public sealed class ServiceUnavailableException : AppException
{
    public ServiceUnavailableException(string message, string errorCode = "SRV_SERVICE_UNAVAILABLE")
        : base(errorCode, StatusCodes.Status503ServiceUnavailable, message) { }
}
