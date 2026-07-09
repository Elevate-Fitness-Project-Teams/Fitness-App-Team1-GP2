using System;

namespace UserProfileService.Domain.Entities
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public bool IsPremiumCached { get; set; }
        public DateTime MemberSince { get; set; }

        // Navigation Properties
        public UserPreferences? Preferences { get; set; }
        public NotificationSettings? NotificationSettings { get; set; }
        public PrivacySettings? PrivacySettings { get; set; }
        public UserStatistics? Statistics { get; set; }
    }
}
