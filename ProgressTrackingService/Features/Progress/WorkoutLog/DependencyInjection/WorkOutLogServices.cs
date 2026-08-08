using ProgressTrackingService.Features.Progress.WorkoutLog.Services;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.DependencyInjection;

public static class WorkOutLogServices
{
    public static IServiceCollection AddWorkOutLogServices(this IServiceCollection services)
    {
        services.AddScoped<WorkoutLogService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<UserStatisticService>();
        services.AddScoped<StreakService>();
        
        return services;
    }
}