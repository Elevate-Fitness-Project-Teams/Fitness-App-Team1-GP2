namespace SmartCoachService.Common.Exceptions;

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
    public NotFoundException(string errorCode, string message) : base(errorCode, StatusCodes.Status404NotFound, message) { }
}

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(string message, string errorCode = "VAL_REQUIRED_FIELD")
        : base(errorCode, StatusCodes.Status400BadRequest, message) { }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string errorCode, string message) : base(errorCode, StatusCodes.Status403Forbidden, message) { }
}

public sealed class ServiceUnavailableException : AppException
{
    public ServiceUnavailableException(string message, string errorCode = "SRV_SERVICE_UNAVAILABLE")
        : base(errorCode, StatusCodes.Status503ServiceUnavailable, message) { }
}
