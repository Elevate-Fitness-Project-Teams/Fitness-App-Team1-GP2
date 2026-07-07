using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.UserProfileService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStatistics",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalWorkouts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatistics", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserStatistics_UserProfiles_UserId",
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
                name: "UserStatistics");
        }
    }
}
