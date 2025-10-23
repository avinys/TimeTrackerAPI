using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Projects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Projects");
        }
    }
}
