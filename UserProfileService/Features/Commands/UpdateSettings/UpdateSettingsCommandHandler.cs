using FitnessApp.Shared.Exceptions;
using FitnessApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;
using UserProfileService.Domain.Entities;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateSettings
{
    public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSettingsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserProfiles.GetQueryable(asNoTracking: false)
                .Include(u => u.Preferences)
                .Include(u => u.NotificationSettings)
                .Include(u => u.PrivacySettings)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new AppException("User profile not found.", System.Net.HttpStatusCode.NotFound, "RES_NOT_FOUND");
            }

            if (request.Request.UserPreferences != null)
            {
                if (user.Preferences == null)
                {
                    user.Preferences = new UserPreferences { UserId = request.UserId };
                }

                var prefs = request.Request.UserPreferences;
                if (prefs.Language != null) user.Preferences.Language = prefs.Language;
                if (prefs.Theme != null) user.Preferences.Theme = prefs.Theme;
                if (prefs.WeightUnit != null) user.Preferences.WeightUnit = prefs.WeightUnit;
                if (prefs.HeightUnit != null) user.Preferences.HeightUnit = prefs.HeightUnit;
                if (prefs.DistanceUnit != null) user.Preferences.DistanceUnit = prefs.DistanceUnit;
            }

            if (request.Request.NotificationSettings != null)
            {
                if (user.NotificationSettings == null)
                {
                    user.NotificationSettings = new NotificationSettings { UserId = request.UserId };
                }

                var notifs = request.Request.NotificationSettings;
                if (notifs.WorkoutReminders.HasValue) user.NotificationSettings.WorkoutReminders = notifs.WorkoutReminders.Value;
                if (notifs.MealReminders.HasValue) user.NotificationSettings.MealReminders = notifs.MealReminders.Value;
                if (notifs.AchievementAlerts.HasValue) user.NotificationSettings.AchievementAlerts = notifs.AchievementAlerts.Value;
                if (notifs.WeeklyReports.HasValue) user.NotificationSettings.WeeklyReports = notifs.WeeklyReports.Value;
                if (notifs.EmailNotifications.HasValue) user.NotificationSettings.EmailNotifications = notifs.EmailNotifications.Value;
                if (notifs.PushNotifications.HasValue) user.NotificationSettings.PushNotifications = notifs.PushNotifications.Value;
            }

            if (request.Request.PrivacySettings != null)
            {
                if (user.PrivacySettings == null)
                {
                    user.PrivacySettings = new PrivacySettings { UserId = request.UserId };
                }

                var priv = request.Request.PrivacySettings;
                if (priv.ProfileVisibility != null) user.PrivacySettings.ProfileVisibility = priv.ProfileVisibility;
                if (priv.ShowProgressToFriends.HasValue) user.PrivacySettings.ShowProgressToFriends = priv.ShowProgressToFriends.Value;
                if (priv.AllowDataSharing.HasValue) user.PrivacySettings.AllowDataSharing = priv.AllowDataSharing.Value;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Success(true, "Settings updated successfully.", 200);
        }
    }
}
