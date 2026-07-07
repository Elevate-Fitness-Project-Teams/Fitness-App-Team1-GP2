namespace FitnessApp.UserProfileService.Features.Queries.GetSettings
{
    public class GetSettingsDto
    {
        public UserPreferencesDto UserPreferences { get; set; } = new();
        public NotificationSettingsDto NotificationSettings { get; set; } = new();
        public PrivacySettingsDto PrivacySettings { get; set; } = new();
    }

    public class UserPreferencesDto
    {
        public string Language { get; set; } = "en";
        public string Theme { get; set; } = "light";
        public string WeightUnit { get; set; } = "kg";
        public string HeightUnit { get; set; } = "cm";
        public string DistanceUnit { get; set; } = "km";
    }

    public class NotificationSettingsDto
    {
        public bool WorkoutReminders { get; set; }
        public bool MealReminders { get; set; }
        public bool AchievementAlerts { get; set; }
        public bool WeeklyReports { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
    }

    public class PrivacySettingsDto
    {
        public string ProfileVisibility { get; set; } = string.Empty;
        public bool ShowProgressToFriends { get; set; }
        public bool AllowDataSharing { get; set; }
    }
}
