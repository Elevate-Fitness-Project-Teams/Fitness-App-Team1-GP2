namespace ProgressTrackingService.Domain.Entities;

public class UserStatistic : BaseEntity<int>
{
    public int UserId { get; set; }
    
    public int TotalWorkouts { get; set; }
    
    public int TotalCaloriesBurned { get; set; }
    
    public decimal TotalWeightLost { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}