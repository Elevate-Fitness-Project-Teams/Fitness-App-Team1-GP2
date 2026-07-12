namespace ProgressTrackingService.Domain.Entities;

public class Achievement : BaseEntity<int> 
{
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string IconUrl { get; set; } = null!;
    
    #region Relationships
    
    public List<UserAchievement> UserAchievements { get; set; } =  new List<UserAchievement>();
    
    #endregion
}