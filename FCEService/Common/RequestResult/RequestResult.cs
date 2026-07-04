namespace FCEService.Common.RequestResult
{
    public class RequestResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public ErrorCode? errorCode { get; set; }

        public static RequestResult<T> Success(T data, string message = "Success")
        {
            return new RequestResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                errorCode = ErrorCode.None
            };
        }

        public static RequestResult<T> Failure(string message = "Request Failed", ErrorCode? errorCode = null)
        {
            return new RequestResult<T>
            {
                IsSuccess = false,
                Data = default,
                Message = message,
                errorCode = errorCode
            };
        }

    }
}
