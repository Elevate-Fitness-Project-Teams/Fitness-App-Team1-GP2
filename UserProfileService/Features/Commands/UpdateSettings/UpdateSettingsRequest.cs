namespace FitnessApp.UserProfileService.Features.Commands.UpdateSettings
{
    public class UpdateSettingsRequest
    {
        public UpdateUserPreferencesDto? UserPreferences { get; set; }
        public UpdateNotificationSettingsDto? NotificationSettings { get; set; }
        public UpdatePrivacySettingsDto? PrivacySettings { get; set; }
    }

    public class UpdateUserPreferencesDto
    {
        public string? Language { get; set; }
        public string? Theme { get; set; }
        public string? WeightUnit { get; set; }
        public string? HeightUnit { get; set; }
        public string? DistanceUnit { get; set; }
    }

    public class UpdateNotificationSettingsDto
    {
        public bool? WorkoutReminders { get; set; }
        public bool? MealReminders { get; set; }
        public bool? AchievementAlerts { get; set; }
        public bool? WeeklyReports { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? PushNotifications { get; set; }
    }

    public class UpdatePrivacySettingsDto
    {
        public string? ProfileVisibility { get; set; }
        public bool? ShowProgressToFriends { get; set; }
        public bool? AllowDataSharing { get; set; }
    }
}
