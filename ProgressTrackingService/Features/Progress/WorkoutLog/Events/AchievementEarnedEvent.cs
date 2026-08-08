namespace ProgressTrackingService.Features.Progress.WorkoutLog.Events;

public class AchievementEarnedEvent
{
    public int UserId { get; set; }
    public string AchievementName { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTimeOffset EarnedAt { get; set; }
}