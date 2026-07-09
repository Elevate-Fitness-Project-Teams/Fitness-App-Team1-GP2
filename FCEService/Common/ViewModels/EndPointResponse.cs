namespace FCEService.Common.ViewModels
{
    public class EndPointResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public ResponseErrorCode? errorCode { get; set; }

        public static EndPointResponse<T> Success(T data, string? message = "Success")
        {
            return new EndPointResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                errorCode = ResponseErrorCode.None
            };
        }
        public static EndPointResponse<T> Success(T data)
        {
            return new EndPointResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = "Success",
                errorCode = ResponseErrorCode.None
            };
        }

        public static EndPointResponse<T> Failure(string message = "Request Failed", ResponseErrorCode? errorCode = null)
        {
            return new EndPointResponse<T>
            {
                IsSuccess = false,
                Data = default,
                Message = message,
                errorCode = errorCode
            };
        }

    }
}
