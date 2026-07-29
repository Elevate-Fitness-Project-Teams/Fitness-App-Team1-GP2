using NutritionService.Common.Middleware;
using NutritionService.Infrastructure;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ----------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<NutritionService.Common.Abstractions.ICurrentUserService, NutritionService.Infrastructure.CurrentUserService>();

builder.Services.AddNutritionInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"] ?? string.Empty))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// ---- Middleware pipeline ------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapNutritionEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "NutritionService" })).AllowAnonymous();

app.Run();
