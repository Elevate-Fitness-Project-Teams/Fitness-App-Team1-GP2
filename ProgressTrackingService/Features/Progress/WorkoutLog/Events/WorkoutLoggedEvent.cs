namespace ProgressTrackingService.Features.Progress.WorkoutLog.Events;

public class WorkoutLoggedEvent
{
    public int UserId { get; set; }
    
    public int WorkoutLogId { get; set; }
    
    public int CaloriesBurned { get; set; }
    
    public DateTimeOffset CaloriesBurnedAt { get; set; }
}