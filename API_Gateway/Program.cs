using System.Threading.RateLimiting;
// using FitnessApp.Shared.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Add CORS policy to allow requests from any origin
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        // Add YARP reverse proxy services
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        // Add JWT authentication
        // builder.Services.AddSharedJwtAuthentication(builder.Configuration);
        
        // Add rate limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 2. تخصيص الرسالة (Custom Response)
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                
                var responseMessage = new
                {
                    Status = 429,
                    Title = "Too Many Requests",
                    Message = "لقد تجاوزت الحد المسموح به من الطلبات (1 طلب كل ثانية). يرجى الانتظار والمحاولة لاحقاً.",
                    AllowedRequests = 1,
                    WindowInMinutes = 1
                };

                await context.HttpContext.Response.WriteAsJsonAsync(responseMessage, cancellationToken);
            };
            
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Request.Headers.Host.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(1),
                        QueueLimit = 0
                    }));
        });
        
        // Add per-user rate limiting policy
        // builder.Services.AddRateLimiter(options =>
        // {
        //     options.AddPolicy("per-user", httpContext =>
        //     {
        //         var userId =
        //             httpContext.User.FindFirst("sub")?.Value
        //             ?? httpContext.Connection.RemoteIpAddress?.ToString()
        //             ?? "anonymous";
        //
        //         return RateLimitPartition.GetFixedWindowLimiter(
        //             userId,
        //             _ => new FixedWindowRateLimiterOptions
        //             {
        //                 PermitLimit = 1,
        //                 Window = TimeSpan.FromSeconds(1),
        //                 QueueLimit = 0
        //             });
        //     });
        // });
        
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        app.UseRouting();
        
        // Use rate limiting middleware
        app.UseRateLimiter();
        
        // Use exception handling middleware
        app.UseHttpsRedirection();

        // Use CORS middleware
        app.UseCors("AllowAll");

        // Use authentication and authorization middleware
        // app.UseAuthentication();
        // app.UseAuthorization();

        // Use YARP reverse proxy middleware
        app.MapReverseProxy();

        // Map controllers
        app.MapControllers();    
        
        app.Run();
    }
}