using ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;

public class ViewWeightHistoryDto
{
    public List<WorkoutLogsDto>? WorkoutLogs { get; set; }
    
    public List<WeightHistoryDto>? WeightHistory { get; set; }
    
    public List<UserStatisticsDto>? UserStatistics { get; set; }
}