using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Behaviors;
using NutritionService.Infrastructure.ExternalServices;
using NutritionService.Infrastructure.Messaging;
using NutritionService.Infrastructure.Persistence;
using Scrutor;
using System.Reflection;

namespace NutritionService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNutritionInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Persistence
        services.AddDbContext<NutritionDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NutritionDb")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // CQRS / MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Messaging
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        // External services (Nutrition -> FCE synchronous call)
        services.AddHttpClient<IFceServiceClient, FceServiceClient>(client =>
        {
            var baseUrl = configuration["Services:FceService:BaseUrl"] ?? "http://fce-service";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Endpoint discovery
        services.Scan(scan => scan.FromAssemblyOf<IEndpoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static void MapNutritionEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(app);
    }
}
