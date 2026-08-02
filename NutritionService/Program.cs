using NutritionService.Common.Middleware;
using NutritionService.Infrastructure;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FitnessApp.Shared.Extensions;
var builder = WebApplication.CreateBuilder(args);

// ---- Services ----------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<NutritionService.Common.Abstractions.ICurrentUserService, NutritionService.Infrastructure.CurrentUserService>();

builder.Services.AddNutritionInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSharedJwtAuthentication(builder.Configuration);
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
