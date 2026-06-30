using System.Net;

namespace AuthenticationService.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        protected AppException(string message, HttpStatusCode statusCode, string errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
