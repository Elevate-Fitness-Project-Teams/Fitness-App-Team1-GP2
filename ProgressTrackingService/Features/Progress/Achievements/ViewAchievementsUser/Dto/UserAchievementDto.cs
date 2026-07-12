namespace ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Dto;

public class UserAchievementDto
{
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string IconUrl { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public DateTimeOffset EarnedAt { get; set; }
}