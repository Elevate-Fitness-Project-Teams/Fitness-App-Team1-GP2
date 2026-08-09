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
                x.AddEntityFrameworkOutbox<AuthDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSection = configuration.GetSection("RabbitMQConnection");

                    cfg.Host(rabbitSection["HostName"], h =>
                    {
                        h.Username(rabbitSection["UserName"] ?? "guest");
                        h.Password(rabbitSection["Password"] ?? "guest");
                    });
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
