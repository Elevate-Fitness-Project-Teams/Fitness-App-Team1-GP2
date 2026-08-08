
namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.ViewModel;

public class ViewProgressUserStatsViewModel
{
    public int UserId { get; set; }
    
    // WorkOutLog
    public int TotalCaloriesBurned { get; set; }
    
    // UserStatistic
    public int TotalWorkouts { get; set; }
    
    public decimal TotalWeightLost { get; set; }
    
    // Streaks
    public int CurrentStreak { get; set; }

    public int LongestStreak { get; set; }
}