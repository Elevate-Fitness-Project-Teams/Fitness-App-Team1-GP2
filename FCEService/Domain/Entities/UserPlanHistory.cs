namespace FCEService.Domain.Entities
{
    public class UserPlanHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public int ExternalPlanId { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? ResonForChange { get; set; }
    }
}
