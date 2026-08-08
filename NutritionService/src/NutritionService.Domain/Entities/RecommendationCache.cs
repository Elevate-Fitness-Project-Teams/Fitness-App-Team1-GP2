namespace SmartCoachService.Domain.Entities;

public class RecommendationCache
{
    public Guid UserId { get; set; }
    public string HomeFeedDataJson { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
