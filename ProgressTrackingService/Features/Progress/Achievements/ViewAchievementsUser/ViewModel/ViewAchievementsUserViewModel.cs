using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Dto;

namespace ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.ViewModel;

public class ViewAchievementsUserViewModel
{
    public List<UserAchievementDto> UserAchievements { get; set; } = new List<UserAchievementDto>();
}