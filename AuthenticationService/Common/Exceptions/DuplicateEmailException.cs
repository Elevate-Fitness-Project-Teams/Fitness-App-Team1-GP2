using System.Net;

namespace AuthenticationService.Common.Exceptions
{
    public class DuplicateEmailException : AppException
    {
        public DuplicateEmailException(string email) 
            : base($"Email '{email}' is already registered.", HttpStatusCode.Conflict, "AUTH_EMAIL_EXISTS")
        {
        }
    }
}
