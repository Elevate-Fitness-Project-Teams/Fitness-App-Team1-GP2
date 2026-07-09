using NotificationService.Common;

public sealed record RequestResult<TResult>(
    TResult? Data,
    bool IsSuccess,
    ErrorCode Error)
{
    public static RequestResult<TResult> Success(TResult data)
        => new(data, true, ErrorCode.None);

    public static RequestResult<TResult> Failure(ErrorCode error)
        => new(default, false, error);
}