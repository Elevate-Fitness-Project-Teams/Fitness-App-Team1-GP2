namespace ProgressTrackingService.Domain.Entities;

public class WorkoutSessionTracking : BaseEntity<int>
{
    public string SessionId { get; set; } = null!;
    
    public int WorkoutId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTimeOffset StartedAt { get; set; }
}