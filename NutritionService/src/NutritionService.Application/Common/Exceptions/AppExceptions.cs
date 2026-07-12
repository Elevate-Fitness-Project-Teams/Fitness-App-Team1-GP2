namespace NutritionService.Application.Common.Exceptions;

/// <summary>Base type so the API middleware can map these to HTTP status + error code uniformly.</summary>
public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string ErrorCode { get; }

    protected AppException(string message) : base(message) { }
}

public sealed class FceMetricsNotCalculatedException : AppException
{
    public override int StatusCode => 400;
    public override string ErrorCode => "FCE_METRICS_NOT_CALCULATED";

    public FceMetricsNotCalculatedException(Guid userId)
        : base($"CalorieTarget has not been calculated yet for user '{userId}'.") { }
}

public sealed class MealNotFoundException : AppException
{
    public override int StatusCode => 404;
    public override string ErrorCode => "RES_MEAL_NOT_FOUND";

    public MealNotFoundException(Guid mealId)
        : base($"Meal '{mealId}' was not found.") { }
}

public sealed class UserNotFoundException : AppException
{
    public override int StatusCode => 404;
    public override string ErrorCode => "RES_USER_NOT_FOUND";

    public UserNotFoundException(Guid userId)
        : base($"User '{userId}' was not found.") { }
}

public sealed class ServiceUnavailableException : AppException
{
    public override int StatusCode => 503;
    public override string ErrorCode => "SERVICE_UNAVAILABLE";

    public ServiceUnavailableException(string message)
        : base(message) { }
}

public sealed class SessionNotFoundException : AppException
{
    public override int StatusCode => 404;
    public override string ErrorCode => "CHAT_SESSION_NOT_FOUND";

    public SessionNotFoundException(Guid sessionId)
        : base($"Chat session '{sessionId}' was not found.") { }
}

public sealed class PremiumRequiredException : AppException
{
    public override int StatusCode => 402;
    public override string ErrorCode => "PREMIUM_REQUIRED";

    public PremiumRequiredException()
        : base("Premium subscription is required to continue using Smart Coach today.") { }
}

public sealed class ValidationAppException : AppException
{
    public override int StatusCode => 400;
    public override string ErrorCode { get; }

    public ValidationAppException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
