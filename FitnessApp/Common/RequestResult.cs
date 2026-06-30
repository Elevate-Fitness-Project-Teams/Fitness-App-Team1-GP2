namespace WorkoutService.Common
{
    public record RequestResult<TResult>(TResult Data, bool IsSuccess, ErrorCode ErrorCode)
    {
        public static RequestResult<TResult> Success(TResult data) => new RequestResult<TResult>(data, true, ErrorCode.None);
        public static RequestResult<TResult> Failure(ErrorCode error) => new RequestResult<TResult>(default!, false, error);
    }

}
