namespace ProgressTrackingService.Domain.Entities;

public class UserAchievement : BaseEntity<int>
{
    public DateTimeOffset AchievedAt { get; set; }
    
    #region Relationships
    
    public int AchievementId { get; set; }
    
    public Achievement? Achievement { get; set; }
    
    public int UserId { get; set; }

    #endregion
}