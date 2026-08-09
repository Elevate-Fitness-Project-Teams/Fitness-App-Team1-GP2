using FitnessApp.Common.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkoutService.Contracts;
using WorkoutService.Database;
using WorkoutService.Messaging;
using WorkoutService.Persistence;

namespace FitnessApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers
            builder.Services.AddControllers();


            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Database
            builder.Services.AddDbContext<WorkoutDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    }
                ));


            // Repository + Unit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            // MediatR
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


            // Fluent Validation
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);



            // ===============================
            // Authentication & Security
            // ===============================


            // if (builder.Environment.IsDevelopment())
            // {
            //     builder.Services
            //         .AddAuthentication(options =>
            //         {
            //             options.DefaultAuthenticateScheme =
            //                 DevelopmentAuthHandler.SchemeName;
            //
            //             options.DefaultChallengeScheme =
            //                 DevelopmentAuthHandler.SchemeName;
            //         })
            //         .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthHandler>(
            //             DevelopmentAuthHandler.SchemeName,
            //             _ => { });
            // }
            // else
            // {
            //     builder.Services
            //         .AddAuthentication(options =>
            //         {
            //             options.DefaultAuthenticateScheme =
            //                 JwtBearerDefaults.AuthenticationScheme;
            //
            //             options.DefaultChallengeScheme =
            //                 JwtBearerDefaults.AuthenticationScheme;
            //         })
            //         .AddJwtBearer(options =>
            //         {
            //             options.TokenValidationParameters =
            //                 new TokenValidationParameters
            //                 {
            //                     ValidateIssuer = true,
            //                     ValidateAudience = true,
            //                     ValidateLifetime = true,
            //                     ValidateIssuerSigningKey = true,
            //
            //                     ValidIssuer =
            //                         builder.Configuration["JWTOptions:Issuer"],
            //
            //                     ValidAudience =
            //                         builder.Configuration["JWTOptions:Audience"],
            //
            //                     IssuerSigningKey =
            //                         new SymmetricSecurityKey(
            //                             Encoding.UTF8.GetBytes(
            //                                 builder.Configuration["JWTOptions:SecretKey"]!))
            //                 };
            //         });
            // }
            
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                builder.Configuration["JWTOptions:Issuer"],

                            ValidAudience =
                                builder.Configuration["JWTOptions:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        builder.Configuration["JWTOptions:SecretKey"]!))
                        };
                });


            // Current User Access
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IUserContext, CurrentUser>();



            // MassTransit + RabbitMQ + EF Outbox

            builder.Services.Configure<RabbitMqOptions>(
          builder.Configuration.GetSection("RabbitMqOptions"));


            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<WorkoutDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Resolve strongly-typed RabbitMqOptions from DI so configuration sources (appsettings, env vars, docker) are honored
                    var options = context.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;
                    var logger = context.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();

                    logger.LogInformation("Configuring RabbitMQ host={Host} port={Port}", options.HostName, options.Port);

                    // Use the overload that accepts host, port and virtual host
                    cfg.Host(
                             options.HostName,
                             "/",
                             h =>
                             {
                                 h.Username(options.UserName);
                                 h.Password(options.Password);
                             });
                    // Retry Policy
                    cfg.UseMessageRetry(retry =>
                    {
                        retry.Interval(
                            3,
                            TimeSpan.FromSeconds(5));
                    });
                    
                    cfg.ConfigureEndpoints(context);
                });
            });


            // Cache
            builder.Services.AddMemoryCache();



            var app = builder.Build();



            // Swagger only Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "swagger";
                });
            }



            app.UseHttpsRedirection();



            // IMPORTANT ORDER
            app.UseAuthentication();

            app.UseAuthorization();



            app.MapControllers();



            app.Run();
        }
    }
}