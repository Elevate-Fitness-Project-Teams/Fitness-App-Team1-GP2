using ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.ViewModel;

public class ViewProgressDashboardViewModel
{
    // weight history
    public List<WeightHistoryDto> WeightHistory { get; set; } = new List<WeightHistoryDto>();
    
    // workout history
    public List<WorkoutLogsDto> WorkoutLogsHistory { get; set; } = new List<WorkoutLogsDto>();
    
    //Achievements
    public List<DashboardUserAchievementDto> UserAchievements { get; set; } = new List<DashboardUserAchievementDto>();
}