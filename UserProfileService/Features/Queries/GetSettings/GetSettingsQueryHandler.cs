using FitnessApp.Shared.Exceptions;
using FitnessApp.Shared.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserProfileService.Domain.Contracts;

namespace FitnessApp.UserProfileService.Features.Queries.GetSettings
{
    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ApiResponse<GetSettingsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSettingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetSettingsDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.UserProfiles.GetProjectedProfileAsync(
                request.UserId,
                u => new GetSettingsDto
                {
                    UserPreferences = u.Preferences == null ? new UserPreferencesDto() : new UserPreferencesDto
                    {
                        Language = u.Preferences.Language,
                        Theme = u.Preferences.Theme,
                        WeightUnit = u.Preferences.WeightUnit,
                        HeightUnit = u.Preferences.HeightUnit,
                        DistanceUnit = u.Preferences.DistanceUnit
                    },
                    NotificationSettings = u.NotificationSettings == null ? new NotificationSettingsDto() : new NotificationSettingsDto
                    {
                        WorkoutReminders = u.NotificationSettings.WorkoutReminders,
                        MealReminders = u.NotificationSettings.MealReminders,
                        AchievementAlerts = u.NotificationSettings.AchievementAlerts,
                        WeeklyReports = u.NotificationSettings.WeeklyReports,
                        EmailNotifications = u.NotificationSettings.EmailNotifications,
                        PushNotifications = u.NotificationSettings.PushNotifications
                    },
                    PrivacySettings = u.PrivacySettings == null ? new PrivacySettingsDto() : new PrivacySettingsDto
                    {
                        ProfileVisibility = u.PrivacySettings.ProfileVisibility,
                        ShowProgressToFriends = u.PrivacySettings.ShowProgressToFriends,
                        AllowDataSharing = u.PrivacySettings.AllowDataSharing
                    }
                },
                cancellationToken
            );

            if (result is null)
            {
                throw new AppException("User settings not found.", System.Net.HttpStatusCode.NotFound, "RES_NOT_FOUND");
            }

            return ApiResponse<GetSettingsDto>.Success(result, "Settings retrieved successfully.", 200);
        }
    }
}
