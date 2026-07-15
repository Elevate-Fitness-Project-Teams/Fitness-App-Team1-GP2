using MediatR;
using MassTransit;
using FluentValidation;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Events;
using ProgressTrackingService.Features.Progress.WorkoutLog.Events;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Infrastructure.DependencyInjection;

public static class InfraStructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // SQL DB
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR -> CQRS
        services.AddMediatR(assembly);
        
        // Fluent Validation
        services.AddValidatorsFromAssembly(assembly);
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        
        // MassTransit
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitSection = configuration.GetSection("RabbitMQConnection");
                
                cfg.Host(rabbitSection["HostName"], h =>
                {
                    h.Username(rabbitSection["UserName"] ?? "guest");
                    h.Password(rabbitSection["Password"] ?? "guest");
                });
                
                // Exchange Name
                cfg.Message<LogWeightEvent>(m => m.SetEntityName("weight-logged-exchange"));
                cfg.Message<WorkoutLoggedEvent>(m => m.SetEntityName("workout-logged-exchange"));
                cfg.Message<AchievementEarnedEvent>(m => m.SetEntityName("achievement-earned-exchange"));
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        // Cloud - Local Redis Cache (Service)
        services.AddStackExchangeRedisCache(options =>
        {
            // First, Use The Cloud, If That Is Not Available, Use The Local
            options.Configuration = $"{configuration.GetConnectionString("RedisConnectionCloud")}, {configuration.GetConnectionString("RedisConnectionLocal")}";
            options.InstanceName = "Caching";
        });
        
        // Redis Services
        services.AddScoped(typeof(RedisServices<>));
        
        return services;
    }
}