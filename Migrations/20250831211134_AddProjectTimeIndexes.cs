using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectTimeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectTimes_UserId_ProjectId_EndTime",
                table: "ProjectTimes",
                columns: new[] { "UserId", "ProjectId", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTimes_UserId_ProjectId_StartTime",
                table: "ProjectTimes",
                columns: new[] { "UserId", "ProjectId", "StartTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectTimes_UserId_ProjectId_EndTime",
                table: "ProjectTimes");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTimes_UserId_ProjectId_StartTime",
                table: "ProjectTimes");
        }
    }
}
