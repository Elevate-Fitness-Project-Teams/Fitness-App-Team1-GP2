namespace AuthenticationService.Domain.Entities
{
    public class OtpCode : BaseEntity
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

    }
}
