using FitnessApp.Shared.Middlewares;
using FitnessApp.Shared.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserProfileService.Infrastructure.Persistence.Context;
using UserProfileService.Domain.Contracts;
using UserProfileService.Infrastructure.Persistence.Repositories;
using FitnessApp.UserProfileService.Domain.Contracts;
using FitnessApp.UserProfileService.Infrastructure.Services;
using FitnessApp.Shared.Extensions;
using FluentValidation;
using FitnessApp.Shared.Models;
using MassTransit;


namespace UserProfileService
{
    public static class Extentions
    {

        public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddSharedJwtAuthentication(configuration);

            services.AddDbContext<UserProfileDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("UserProfileConnection")));

            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserProfileService.Features.Consumers.UserRegisteredConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            services.AddSharedSwagger();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();


            return services;
        }


    }
}
