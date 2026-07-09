
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Notifications.GetNotifications;
using NotificationService.Domain.Notifications;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence.Repositories;

namespace NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<Infrastructure.Persistence.NotificationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped<INotificationRepository,NotificationRepository>();
            builder.Services.AddMemoryCache();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            builder.Services.AddValidatorsFromAssembly(typeof(GetNotificationsValidator).Assembly);
            builder.Services.Configure<RabbitMqOptions>(
               builder.Configuration.GetSection("RabbitMQ"));

            builder.Services.AddHostedService<AchievementEarnedConsumer>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
