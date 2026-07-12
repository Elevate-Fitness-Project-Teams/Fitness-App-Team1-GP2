using FitnessApp.Shared.Middlewares;
using FitnessApp.Shared.Models;
using FitnessApp.Shared.Behaviors;
using FitnessApp.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationService.infrastructure.Persistence.Context;
using AuthenticationService.Domain.Contracts;
using AuthenticationService.infrastructure.Persistence.Repositories;
using AuthenticationService.infrastructure.Security;
using AuthenticationService.Features.Login;

using MassTransit;

namespace AuthenticationService
{
    public static class Extentions
    {

        public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddSharedJwtAuthentication(configuration);

            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthenticationConnection")));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddMemoryCache();
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHttpContextAccessor();

            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            services.AddScoped<ILoginManager, LoginManager>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }




    }
}
