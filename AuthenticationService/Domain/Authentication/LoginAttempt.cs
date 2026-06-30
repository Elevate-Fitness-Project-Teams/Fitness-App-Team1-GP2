namespace AuthenticationService.Domain.Authentication
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime AttemptedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string IpAddress { get; set; }
    }
}
