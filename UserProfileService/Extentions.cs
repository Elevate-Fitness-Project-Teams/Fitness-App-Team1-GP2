using FitnessApp.Shared.Middlewares;
using FitnessApp.Shared.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserProfileService.Infrastructure.Persistence.Context;
using UserProfileService.Domain.Contracts;
using UserProfileService.Infrastructure.Persistence.Repositories;
using FitnessApp.Shared.Extensions;
using FluentValidation;
using FitnessApp.Shared.Models;


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


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            services.AddSharedSwagger();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();


            return services;
        }


    }
}
