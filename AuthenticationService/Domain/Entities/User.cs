namespace AuthenticationService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsLockedOut { get; set; } = false;
        public DateTime? LockedUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool ProfileCompleted { get; set; } = false;
    }
}
