using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<WorkoutLog> WorkoutLogs { get; set; }
    
    public DbSet<Achievement> Achievements { get; set; }
    
    public DbSet<BodyMeasurement> BodyMeasurements { get; set; }
    
    public DbSet<Streak> Streaks { get; set; }
    
    public DbSet<UserAchievement> UserAchievements { get; set; }
    
    public DbSet<UserStatistic> UserStatistics { get; set; }
    
    public DbSet<WeightHistory> WeightHistories { get; set; }
    
    public DbSet<WorkoutLogExercise> WorkoutLogExercises { get; set; }
}