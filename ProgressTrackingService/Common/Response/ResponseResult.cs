namespace ProgressTrackingService.Common.Response;

public record ResponseResult<T>(T Data, bool IsSuccess, string Message, StatusCode StatusCode)
{
    public static ResponseResult<T> Success(T data, string message, StatusCode noStatus)
        => new(data, true, message, StatusCode.Success);
    
    public static ResponseResult<T> Failure(StatusCode statusCode, string message)
        => new(default!, false, message, statusCode);
}
