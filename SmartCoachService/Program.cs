using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartCoachService.Common.Middleware;
using SmartCoachService.Infrastructure;
using System.Text;
using FitnessApp.Shared.Extensions;
var builder = WebApplication.CreateBuilder(args);

// ---- Services ----------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SmartCoachService.Common.Abstractions.ICurrentUserService, SmartCoachService.Infrastructure.CurrentUserService>();

builder.Services.AddSmartCoachInfrastructure(builder.Configuration);

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

app.MapSmartCoachEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "SmartCoachService" })).AllowAnonymous();

app.Run();
