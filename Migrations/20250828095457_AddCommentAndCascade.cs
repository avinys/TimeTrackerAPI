using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAndCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ProjectTimes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Correction",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTimes_ProjectId",
                table: "ProjectTimes",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTimes_Projects_ProjectId",
                table: "ProjectTimes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTimes_Projects_ProjectId",
                table: "ProjectTimes");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTimes_ProjectId",
                table: "ProjectTimes");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ProjectTimes");

            migrationBuilder.DropColumn(
                name: "Correction",
                table: "Projects");
        }
    }
}
