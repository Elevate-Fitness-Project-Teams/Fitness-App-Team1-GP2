namespace ProgressTrackingService.Features.Progress.WorkoutLog.Dto;

public class WorkoutLogDto
{
    public int WorkoutLogLogId { get; set; }

    public bool StreakUpdated { get; set; }

    public int CurrentStreak { get; set; }
}