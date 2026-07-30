using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Behaviors;
using SmartCoachService.Infrastructure.Caching;
using SmartCoachService.Infrastructure.ExternalServices;
using SmartCoachService.Infrastructure.Messaging;
using SmartCoachService.Infrastructure.Persistence;
using Scrutor;
using System.Reflection;

namespace SmartCoachService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSmartCoachInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Persistence
        services.AddDbContext<SmartCoachDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SmartCoachDb")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // CQRS / MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Caching (free-tier rate limit + home feed short circuit reads)
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));
        services.AddScoped<ICacheService, RedisCacheService>();

        // Messaging
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<RecommendationCacheInvalidationConsumer>();

        // External services used by the Home Feed aggregator + chat context builder
        services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>(ConfigureClient(configuration, "ProfileService"));
        services.AddHttpClient<IFceServiceClient, FceServiceClient>(ConfigureClient(configuration, "FceService"));
        services.AddHttpClient<IWorkoutServiceClient, WorkoutServiceClient>(ConfigureClient(configuration, "WorkoutService"));
        services.AddHttpClient<INutritionServiceClient, NutritionServiceClient>(ConfigureClient(configuration, "NutritionService"));
        services.AddHttpClient<IProgressServiceClient, ProgressServiceClient>(ConfigureClient(configuration, "ProgressService"));
        services.AddHttpClient<IAiCoachClient, AiCoachClient>(ConfigureClient(configuration, "AiCoachProvider"));

        // Endpoint discovery
        services.Scan(scan => scan.FromAssemblyOf<IEndpoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    private static Action<HttpClient> ConfigureClient(IConfiguration configuration, string sectionName) => client =>
    {
        var baseUrl = configuration[$"Services:{sectionName}:BaseUrl"] ?? $"http://{sectionName.ToLowerInvariant()}";
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(8);
    };

    public static void MapSmartCoachEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(app);
    }
}
