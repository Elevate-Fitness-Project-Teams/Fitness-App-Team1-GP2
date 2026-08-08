using NotificationService.Common;

namespace NotificationService.Application.Common;

public sealed record ApiResponse<T>(
    bool IsSuccess,
    string Message,
    T? Data,
    List<string> Errors,
    int StatusCode,
    DateTime Timestamp)
{
    public static ApiResponse<T> Success(
        T? data,
        string message = "Operation completed successfully.",
        int statusCode = 200)
    {
        return new ApiResponse<T>(
            true,
            message,
            data,
            [],
            statusCode,
            DateTime.UtcNow
        );
    }

    public static ApiResponse<T> Failure(
        ErrorCode error,
        List<string>? errors = null)
    {
        var statusCode = GetStatusCode(error);

        return new ApiResponse<T>(
            false,
            GetMessage(error),
            default,
            errors ?? [error.ToString()],
            statusCode,
            DateTime.UtcNow
        );
    }

    private static int GetStatusCode(ErrorCode error)
    {
        return error switch
        {
            ErrorCode.ValidationError => 400,
            ErrorCode.Unauthorized => 401,
            ErrorCode.NotificationNotFound => 404,
            _ => 500
        };
    }

    private static string GetMessage(ErrorCode error)
    {
        return error switch
        {
            ErrorCode.ValidationError => "Validation error.",
            ErrorCode.Unauthorized => "Unauthorized.",
            ErrorCode.NotificationNotFound => "Notification was not found.",
            _ => "An unexpected error occurred."
        };
    }
}