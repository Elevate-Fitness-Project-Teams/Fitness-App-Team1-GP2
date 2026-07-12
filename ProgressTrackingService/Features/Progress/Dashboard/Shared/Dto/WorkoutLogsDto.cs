using ProgressTrackingService.Domain.Enums;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;

public class WorkoutLogsDto
{
    public int DurationInMinutes { get; set; }
    
    public int CaloriesBurned { get; set; }

    public Rating Rating { get; set; }
    
    public string? Notes { get; set; } = string.Empty;
    
    public DateTimeOffset CompletedAt { get; set; }
    
    public DifficultyLevel DifficultyLevel { get; set; }
    
    // FK
    public int UserId { get; set; }
    
    // FK
    public int WorkoutId { get; set; }
    
    public List<WorkoutLogExerciseDto> WorkoutLogExercises { get; set; } = new List<WorkoutLogExerciseDto>();
    
    // FK
    public string SessionId { get; set; } = string.Empty;
}