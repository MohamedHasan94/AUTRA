using Microsoft.EntityFrameworkCore.Migrations;

namespace AUTRA.Data.Migrations
{
    public partial class projectName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Project",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                columns: new[] { "Fk_UserId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Project",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                columns: new[] { "Fk_UserId", "Path" });
        }
    }
}
