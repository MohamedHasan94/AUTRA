using Microsoft.EntityFrameworkCore.Migrations;

namespace AUTRA.Data.Migrations
{
    public partial class addProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Fk_UserId = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => new { x.Fk_UserId, x.Path });
                    table.ForeignKey(
                        name: "FK_Project_AspNetUsers_Fk_UserId",
                        column: x => x.Fk_UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
