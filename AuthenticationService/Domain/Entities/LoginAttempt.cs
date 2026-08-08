namespace AuthenticationService.Domain.Entities
{
    public class LoginAttempt : BaseEntity
    {
        public string Email { get; set; }
        public DateTime AttemptedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string IpAddress { get; set; }
    }
}
