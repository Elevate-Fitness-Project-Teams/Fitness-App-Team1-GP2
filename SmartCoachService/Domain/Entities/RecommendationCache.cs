using SmartCoachService.Common.Abstractions;

namespace SmartCoachService.Domain.Entities;

/// <summary>Cache-first edge-aggregator entry backing GET /api/v1/home (User Story 7.3).</summary>
public sealed class RecommendationCache : BaseEntity
{
    public Guid UserId { get; set; }
    public string HomeFeedDataJson { get; set; } = "{}";
    public DateTime ExpiresAt { get; set; }
}
