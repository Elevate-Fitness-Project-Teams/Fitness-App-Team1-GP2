using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NutritionService.Api;
using NutritionService.Api.Middleware;
using NutritionService.Application.Common.Abstractions;
using NutritionService.Domain.Common.Interfaces;
using NutritionService.Infrastructure.Messaging;
using NutritionService.Infrastructure.Persistence;
using RabbitMQ.Client;
using SmartCoachService.Application.Common.Interfaces;
using SmartCoachService.Infrastructure.ExternalServices;
using SmartCoachService.Infrastructure.Messaging;
using SmartCoachService.Infrastructure.Persistence;
using NutritionCurrentUserService = NutritionService.Application.Common.Abstractions.ICurrentUserService;
using SmartCoachCurrentUserService = SmartCoachService.Application.Common.Interfaces.ICurrentUserService;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var dataProtectionBuilder = builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")));
if (OperatingSystem.IsWindows())
{
    dataProtectionBuilder.ProtectKeysWithDpapi();
}

// ---------- MVC / Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var serviceNames = new[] { "ProfileService", "FceService", "WorkoutService", "NutritionService", "ProgressService" };
foreach (var serviceName in serviceNames)
{
    builder.Services.AddHttpClient(serviceName, client =>
    {
        var baseUrl = builder.Configuration[$"Services:{serviceName}:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            client.BaseAddress = new Uri(baseUrl);
        }
    });
}

// ---------- CQRS (MediatR) ----------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(NutritionService.Application.Features
        .GetMealRecommendations.Queries.GetMealRecommendationsQuery).Assembly));

// ---------- Persistence ----------
builder.Services.AddDbContext<NutritionDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("NutritionDb")));

builder.Services.AddDbContext<SmartCoachDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("NutritionDb")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAiCoachService, AiCoachService>();
builder.Services.AddScoped<IHomeFeedAggregator, HomeFeedAggregator>();

// ---------- Current user (from JWT) ----------
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<NutritionCurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<SmartCoachCurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());

// ---------- RabbitMQ ----------
if (builder.Configuration.GetValue<bool>("RabbitMq:Enabled"))
{
    builder.Services.AddSingleton<IConnection>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMq:Host"] ?? "localhost",
            Port = int.TryParse(config["RabbitMq:Port"], out var p) ? p : 5672,
            UserName = config["RabbitMq:Username"] ?? "guest",
            Password = config["RabbitMq:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };
        return factory.CreateConnection("nutrition-service");
    });
    builder.Services.AddHostedService<CalorieTargetCalculatedConsumer>();
    builder.Services.AddHostedService<FceContextConsumer>();
    builder.Services.AddHostedService<ProgressContextConsumer>();
}

// ---------- Auth ----------
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
