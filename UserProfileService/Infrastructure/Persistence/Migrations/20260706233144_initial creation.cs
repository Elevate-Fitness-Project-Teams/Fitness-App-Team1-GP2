using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.UserProfileService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initialcreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    IsPremiumCached = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MemberSince = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkoutReminders = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MealReminders = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AchievementAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    WeeklyReports = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PushNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivacySettings",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProfileVisibility = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "private"),
                    ShowProgressToFriends = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AllowDataSharing = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacySettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PrivacySettings_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Theme = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, defaultValue: "light"),
                    WeightUnit = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, defaultValue: "kg"),
                    HeightUnit = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, defaultValue: "cm"),
                    DistanceUnit = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, defaultValue: "km")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "PrivacySettings");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
