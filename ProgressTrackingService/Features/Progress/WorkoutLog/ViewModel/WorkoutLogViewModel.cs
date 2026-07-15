namespace ProgressTrackingService.Features.Progress.WorkoutLog.ViewModel;

public class WorkoutLogViewModel
{
    public int WorkoutLogLogId { get; set; }

    public bool StreakUpdated { get; set; }

    public int CurrentStreak { get; set; }
}