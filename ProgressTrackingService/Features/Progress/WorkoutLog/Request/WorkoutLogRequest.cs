using ProgressTrackingService.Domain.Enums;
using ProgressTrackingService.Features.Progress.WorkoutLog.Dto;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Request;

public class WorkoutLogRequest
{
    public int WorkoutId { get; set; }
    
    public string SessionId { get; set; } = null!;
    
    public DateTimeOffset CompletedAt { get; set; }
    
    public int Duration { get; set; } 
    
    public int CaloriesBurned { get; set; }
    
    public DifficultyLevel  Difficulty { get; set; }
    
    public string? Notes { get; set; }
    
    public Rating Rating { get; set; }
    
    public List<ExerciseCompletedDto> ExercisesCompleted { get; set; } = new List<ExerciseCompletedDto>();
}