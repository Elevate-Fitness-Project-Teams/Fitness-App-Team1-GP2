namespace ProgressTrackingService.Common.Response;

public record ApiResponse<T>(T Response, bool IsSuccess, string Message, StatusCode StatusCode)
{
    public static ApiResponse<T> Success(T response, string message, StatusCode noStatus)
        => new(response, true, message, StatusCode.Success);
    
    public static ApiResponse<T> Failure(string message, StatusCode statusCode)
        => new(default!, false, message, statusCode);
}