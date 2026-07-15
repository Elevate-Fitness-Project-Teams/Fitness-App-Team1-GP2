using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressTrackingService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProgressTracking");

            migrationBuilder.CreateTable(
                name: "Achievements",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BodyMeasurements",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Neck = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Chest = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Biceps = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Waist = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Hips = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Thighs = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    RecordedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Streaks",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LongestStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastWorkoutDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStatistics",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalWorkouts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalCaloriesBurned = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalWeightLost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeightHistories",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutLogs",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Rating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkoutId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AchievedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalSchema: "ProgressTracking",
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutLogExercises",
                schema: "ProgressTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetsCompleted = table.Column<int>(type: "int", nullable: false),
                    RepsCompleted = table.Column<int>(type: "int", nullable: false),
                    WeightUsed = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false, defaultValue: 0m),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    WorkoutLogId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutLogExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutLogExercises_WorkoutLogs_WorkoutLogId",
                        column: x => x.WorkoutLogId,
                        principalSchema: "ProgressTracking",
                        principalTable: "WorkoutLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Name",
                schema: "ProgressTracking",
                table: "Achievements",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_UserId",
                schema: "ProgressTracking",
                table: "BodyMeasurements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Streaks_UserId",
                schema: "ProgressTracking",
                table: "Streaks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementId",
                schema: "ProgressTracking",
                table: "UserAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId",
                schema: "ProgressTracking",
                table: "UserAchievements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStatistics_UserId",
                schema: "ProgressTracking",
                table: "UserStatistics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightHistories_UserId",
                schema: "ProgressTracking",
                table: "WeightHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogExercises_ExerciseId",
                schema: "ProgressTracking",
                table: "WorkoutLogExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogExercises_WorkoutLogId",
                schema: "ProgressTracking",
                table: "WorkoutLogExercises",
                column: "WorkoutLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_CompletedAt",
                schema: "ProgressTracking",
                table: "WorkoutLogs",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_SessionId",
                schema: "ProgressTracking",
                table: "WorkoutLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_UserId",
                schema: "ProgressTracking",
                table: "WorkoutLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutLogs_UserId_CompletedAt",
                schema: "ProgressTracking",
                table: "WorkoutLogs",
                columns: new[] { "UserId", "CompletedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyMeasurements",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "Streaks",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "UserAchievements",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "UserStatistics",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "WeightHistories",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "WorkoutLogExercises",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "Achievements",
                schema: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "WorkoutLogs",
                schema: "ProgressTracking");
        }
    }
}
