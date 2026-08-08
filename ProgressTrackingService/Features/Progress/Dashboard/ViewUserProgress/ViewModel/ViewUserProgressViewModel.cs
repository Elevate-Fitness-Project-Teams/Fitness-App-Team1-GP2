using ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.ViewModel;

public class ViewUserProgressViewModel
{
    public List<WorkoutLogsDto>? WorkoutLogs { get; set; }
    
    public List<WeightHistoryDto>? WeightHistory { get; set; }
    
    public List<UserStatisticsDto>? UserStatistics { get; set; }
}